/*
	This file is part of L Aerospace Shared Libraries / KSP Division (LASL-KSP)
		© 2023 LisiasT : http://lisias.net <support@lisias.net>

	CrewLight is double licensed, as follows:
		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	And you are allowed to choose the License that better suit your needs.

	LASL-KSP is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the SKL Standard License 1.0
	along with LASL-KSP.
	If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

	You should have received a copy of the GNU General Public License 2.0
	along with LASL-KSP.
	If not, see <https://www.gnu.org/licenses/>.

*/
using TargetModule = global::AviationLights.ModuleNavLight;

namespace LASL.KSP.Support.SwitchLights.AviationLights
{
	public class AviationLights : ISwitchLights
	{
		public AviationLights () { }
		~AviationLights() { }
		void System.IDisposable.Dispose() { }

		private static string[] moduleNames = new string[] { "ModuleNavLight" };
		string[] ISwitchLights.ModuleNames => moduleNames;

		bool ISwitchLights.IsActive(PartModule pm) => true;
		bool ISwitchLights.IsSlowLight(PartModule pm, float thresholdInSecs) => false;

		bool ISwitchLights.IsBeaconLight(PartModule pm)
		{
			bool r = pm.part.name.StartsWith("lightbeacon_");
			r = r || this.IsFromPreset("beacon", pm);
			return r;
		}

		bool ISwitchLights.IsNavigationLight(PartModule pm)
		{
			bool r = pm.part.name.StartsWith("lightnav_");
			r = r || this.IsFromPreset("nav", pm);
			return r;
		}

		bool ISwitchLights.IsStrobeLight(PartModule pm)
		{
			bool r = pm.name.StartsWith("lightstrobe_");
			r = r || this.IsFromPreset("strobe", pm);
			return r;
		}

		bool ISwitchLights.IsUtilityLight(PartModule pm) => false;

		private bool IsFromPreset(string name, PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			return name.Equals(tm.typePreset);
		}

		bool ISwitchLights.IsOn(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			Log.dbg("Part Module {0} is {1}", tm, tm.navLightSwitch);
			return 0 != tm.navLightSwitch;
		}

		void ISwitchLights.TurnOn(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			tm.navLightSwitch = tm.toggleModeSelector;
			Log.dbg("Part Module {0} was turned on", tm);
		}

		void ISwitchLights.TurnOff(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			tm.navLightSwitch = 0;
			Log.dbg("Part Module {0} was turned off", tm);
		}
	}
}
