/*
	This file is part of L Aerospace Shared Libraries / KSP Division (LASL-KSP)
		© 2023 LisiasT : http://lisias.net <support@lisias.net>

	CrewLight is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	Crew Light /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with Crew Light /L Unleashed.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with Crew Light /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Collections.Generic;

namespace LASL.KSP.Support.SwitchLights
{
	public interface ISettings : IDisposable
	{
		int maxPartsToUse { get; }
		float thresholdInSecs { get; }
	}

	public class Regime
	{
		public readonly string Name;
		public readonly bool IgnoreKerbals;
		public readonly bool IgnoreLandingGears;
		public readonly bool IgnoreCrewableParts;
		public readonly bool IgnoreSlowLights;

		public Regime(string name, bool ignoreKerbals
			, bool ignoreLandingGears, bool ignoreCrewableParts, bool ignoreSlowLights
		)
		{
			this.Name = name;
			this.IgnoreKerbals = ignoreKerbals;
			this.IgnoreLandingGears = ignoreLandingGears;
			this.IgnoreCrewableParts = ignoreCrewableParts;
			this.IgnoreSlowLights = ignoreSlowLights;
		}
	}

	public interface ISwitchLights : IDisposable
	{
		string[] ModuleNames { get; }

		bool IsActive(PartModule pm);

		bool IsSlowLight(PartModule pm, float thresholdInSecs);
		bool IsBeaconLight(PartModule pm);
		bool IsNavigationLight(PartModule pm);
		bool IsStrobeLight(PartModule pm);
		bool IsUtilityLight(PartModule pm);

		bool IsOn(PartModule pm);
		void TurnOn(PartModule pm);
		void TurnOff(PartModule pm);
	}

	public interface IVesselFeature : IDisposable
	{
		void TurnOn(Regime regime = null);
		void TurnOn(List<Part> modulesLight);
		void TurnOff(Regime regime = null);
		void TurnOff(List<Part> modulesLight);
		void PushState();
		void PopState();
		List<Part> PartsWithLight();
	}

	public interface IPartFeature : IDisposable
	{
		string id { get; }

		bool IsActive();

		bool IsSlowLight();
		bool IsBeaconLight();
		bool IsNavigationLight();
		bool IsStrobeLight();
		bool IsUtilityLight();

		bool IsOn();
		void TurnOn(Regime regime = null);
		void TurnOff(Regime regime = null);
	}

	public class Controller
	{
		public static readonly Regime ALLPARTS = new Regime("All Parts", false, false, false, false);

		private class ModuleCache
		{
			private readonly Dictionary<string,ISwitchLights> modules = new Dictionary<string, ISwitchLights>();
			private readonly string[] moduleKeys;
			public string[] Modules => moduleKeys;

			private class Dummy : ISwitchLights
			{
				private static readonly string[] moduleNames = new string[] { };
				string[] ISwitchLights.ModuleNames => moduleNames;

				void System.IDisposable.Dispose() { }

				bool ISwitchLights.IsActive(PartModule pm) => false;
				bool ISwitchLights.IsSlowLight(PartModule pm, float thresholdInSecs) => true;

				bool ISwitchLights.IsBeaconLight(PartModule pm) => false;
				bool ISwitchLights.IsNavigationLight(PartModule pm) => false;
				bool ISwitchLights.IsStrobeLight(PartModule pm) => false;
				bool ISwitchLights.IsUtilityLight(PartModule pm) => false;

				bool ISwitchLights.IsOn(PartModule pm) => false;
				void ISwitchLights.TurnOff(PartModule pm) { }
				void ISwitchLights.TurnOn(PartModule pm) { }
			}
			private static readonly Dummy DUMMY = new Dummy();

			internal ModuleCache()
			{
				foreach (Type t in KSPe.Util.SystemTools.Type.Search.By(typeof(ISwitchLights)))
				{
					Log.dbg("Found Interface client {0}", t.FullName);
					ISwitchLights o = (ISwitchLights)System.Activator.CreateInstance(t);
					Log.dbg("Intantiated object: {0} {1}", o, string.Join(", ", o.ModuleNames));
					foreach (string s in o.ModuleNames)
						this.modules[s] = o;
				}
				string[] r = new string[this.modules.Keys.Count];
				this.modules.Keys.CopyTo(r, 0);
				this.moduleKeys = r;
				Log.dbg("Suitable modules found: {0} {1}", this.moduleKeys.Length, string.Join(", ", this.moduleKeys));
			}

			~ModuleCache()
			{
				this.modules.Clear();
			}

			public bool Supports(PartModule pm)
			{
				int i = moduleKeys.IndexOf<string>(pm.moduleName);
				return i > -1;
			}

			public ISwitchLights this[PartModule index]
			{
				get
				{
					if (null == index) return DUMMY;
					if (!this.modules.ContainsKey(index.moduleName)) return DUMMY;
					return this.modules[index.moduleName];
				}
			}
		}

		private readonly Dictionary<Vessel,VesselImplementation> vessels = new Dictionary<Vessel,VesselImplementation>();
		private readonly ModuleCache modules;
		private readonly ISettings settings;

		private class PartImplementation : IPartFeature
		{
			private readonly ISettings settings;
			private readonly Part part;
			private readonly PartModule targetModule;
			private readonly ISwitchLights ifc;
			private readonly bool isKerbal;
			private readonly bool isLangingGear;
			private readonly bool isCrewable;

			internal PartImplementation(ISettings settings, ModuleCache modules, Part part)
			{
				this.settings = settings;
				this.part = part;
				foreach (PartModule pm in part.Modules) if (modules.Supports(pm))
				{
					this.targetModule = pm;
					this.ifc = modules[pm];
					break;
				}
				if (null == this.ifc) this.ifc = modules[null];
				this.isKerbal = this.part.Modules.Contains<KerbalEVA>();
				this.isLangingGear = this.part.Modules.Contains<ModuleStatusLight>();
				this.isCrewable = this.part.CrewCapacity > 0;
			}
			~PartImplementation() { }
			void System.IDisposable.Dispose() { }

			private string _id = null;
			string IPartFeature.id => this._id ?? (this._id = this.part.vessel.GetInstanceID().ToString() + ":" + this.part.GetInstanceID().ToString());

			bool IPartFeature.IsActive() => this.ifc.IsActive(this.targetModule);

			bool IPartFeature.IsSlowLight() => this.ifc.IsSlowLight(this.targetModule, this.settings.thresholdInSecs);
			bool IPartFeature.IsBeaconLight() => this.ifc.IsBeaconLight(this.targetModule);
			bool IPartFeature.IsNavigationLight() => this.ifc.IsNavigationLight(this.targetModule);
			bool IPartFeature.IsStrobeLight() => this.ifc.IsStrobeLight(this.targetModule);
			bool IPartFeature.IsUtilityLight() => this.ifc.IsUtilityLight(this.targetModule);

			bool IPartFeature.IsOn() => this.ifc.IsOn(this.targetModule);

			void IPartFeature.TurnOff(Regime regime)
			{
				regime = regime ?? ALLPARTS;
				if (
						regime.IgnoreKerbals && this.isKerbal
					||	regime.IgnoreLandingGears && this.isLangingGear
					||	regime.IgnoreCrewableParts && this.isCrewable
					||	regime.IgnoreSlowLights && this.ifc.IsSlowLight(this.targetModule, this.settings.thresholdInSecs)
				) return;
				this.ifc.TurnOff(this.targetModule);
			}

			void IPartFeature.TurnOn(Regime regime)
			{
				regime = regime ?? ALLPARTS;
				if (
						regime.IgnoreKerbals && this.isKerbal
					||	regime.IgnoreLandingGears && this.isLangingGear
					||	regime.IgnoreCrewableParts && this.isCrewable
					||	regime.IgnoreSlowLights && this.ifc.IsSlowLight(this.targetModule, this.settings.thresholdInSecs)
				) return;
				this.ifc.TurnOn(this.targetModule);
			}
		}

		private class VesselImplementation : IVesselFeature
		{
			private class Dummy : IPartFeature
			{
				void System.IDisposable.Dispose() { }

				public string id => "dummy";
				bool IPartFeature.IsActive() => false;
				bool IPartFeature.IsSlowLight() => true;

				bool IPartFeature.IsBeaconLight() => false;
				bool IPartFeature.IsNavigationLight() => false;
				bool IPartFeature.IsStrobeLight() => false;
				bool IPartFeature.IsUtilityLight() => false;

				bool IPartFeature.IsOn() => false;
				void IPartFeature.TurnOff(Regime regime) { }
				void IPartFeature.TurnOn(Regime regime) { }
			}
			private readonly Dummy DUMMY = new Dummy();

			private readonly ISettings settings;
			private readonly ModuleCache modules;
			private readonly Vessel vessel;
			private readonly Dictionary<Part,IPartFeature> parts = new Dictionary<Part,IPartFeature>();
			private readonly Dictionary<string, bool> stack = new Dictionary<string, bool>();
			private bool needsInitialization = true;

			internal VesselImplementation(ISettings settings, ModuleCache modules, Vessel v)
			{
				this.settings = settings;
				this.modules = modules;
				this.vessel = v;
			}
			~VesselImplementation()
			{
				if (!this.disposed) (this as IDisposable).Dispose();
			}
			private bool disposed = false;
			void System.IDisposable.Dispose()
			{
				this.settings.Dispose();

				this.stack.Clear();

				foreach (IPartFeature i in this.parts.Values)
					i.Dispose();
				this.parts.Clear();

				this.disposed = true;
			}

			internal IPartFeature this[Part p]
			{
				get
				{
					Log.dbg("Getting IPartFeature for {0}", p);
					if (!this.parts.ContainsKey(p))
					{
						if (!this.needsInitialization) return DUMMY;
						if (this.parts.Count > this.settings.maxPartsToUse) return DUMMY;
						this.parts[p] = new PartImplementation(this.settings, this.modules, p);
					}
					return this.parts[p];
				}
			}

			void IVesselFeature.TurnOff(Regime regime)
			{
				if (this.needsInitialization) this.init();
				foreach (IPartFeature p in this.parts.Values) if (p.IsActive())
					p.TurnOff(regime);
			}

			void IVesselFeature.TurnOn(Regime regime)
			{
				if (this.needsInitialization) this.init();
				foreach (IPartFeature p in this.parts.Values) if (p.IsActive())
					p.TurnOn(regime);
			}

			void IVesselFeature.PushState()
			{
				if (this.needsInitialization) this.init();
				this.stack.Clear();
				foreach (IPartFeature p in this.parts.Values)
					this.stack[p.id] = p.IsOn();
			}

			void IVesselFeature.PopState()
			{
				if (this.needsInitialization) this.init();
				foreach (IPartFeature p in this.parts.Values) if (this.stack.ContainsKey(p.id))
					if (this.stack[p.id])
						p.TurnOn();
					else
						p.TurnOff();
			}

			void IVesselFeature.TurnOn(List<Part> list)
			{
				foreach (Part p in list)
					this[p].TurnOn();
			}

			void IVesselFeature.TurnOff(List<Part> list)
			{
				foreach (Part p in list)
					this[p].TurnOff();
			}

			List<Part> IVesselFeature.PartsWithLight()
			{
				if (this.needsInitialization) this.init();
				return new List<Part>(this.parts.Keys);
			}

			private void init()
			{
				foreach (Part p in this.vessel.parts)
					if (!this.parts.ContainsKey(p))
						if (this.vessel.parts.Count <= this.settings.maxPartsToUse)
							this.parts[p] = new PartImplementation(this.settings, this.modules, p);
						else
							break;
				this.needsInitialization = false;
			}
		}

		public Controller(ISettings settings)
		{
			this.settings = settings;
			this.modules = new ModuleCache();
			GameEvents.onVesselChange.Add(this.OnVesselChange);
			GameEvents.onVesselDestroy.Add(this.OnVesselDestroy);
		}

		~Controller()
		{
			GameEvents.onVesselDestroy.Remove(this.OnVesselDestroy);
			GameEvents.onVesselChange.Remove(this.OnVesselChange);
			this.vessels.Clear();
			this.settings.Dispose();
		}

		public IVesselFeature this[Vessel v]
		{
			get
			{
				Log.dbg("Getting IVesselFeature for {0}", v);
				if (!this.vessels.ContainsKey(v)) this.vessels[v] = new VesselImplementation(this.settings, this.modules, v);
				return this.vessels[v];
			}
		}

		public IPartFeature this[Part p]
		{
			get
			{
				Log.dbg("Getting IPartFeature for {0}", p);
				if (!this.vessels.ContainsKey(p.vessel)) this.vessels[p.vessel] = new VesselImplementation(this.settings, this.modules, p.vessel);
				return this.vessels[p.vessel][p];
			}
		}

		public bool Supports(Part part)
		{
			foreach (string m in this.modules.Modules) if (part.Modules.Contains(m))
				return true;
			return false;
		}

		private void OnVesselDestroy(Vessel data) => this.clear(data);
		private void OnVesselChange(Vessel data) => this.clear(data);

		private void clear(Vessel v)
		{
			if (!this.vessels.ContainsKey(v)) return;
			this.vessels.Remove(v);
		}
	}
}
