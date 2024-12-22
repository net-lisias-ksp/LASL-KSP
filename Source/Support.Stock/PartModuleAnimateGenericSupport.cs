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
using System.Text.RegularExpressions;

using TargetModule = global::ModuleAnimateGeneric;

namespace LASL.KSP.Support.SwitchLights.Stock
{
	public class AnimateGeneric : ISwitchLights
	{
		public AnimateGeneric() { }
		~AnimateGeneric() { }
		void System.IDisposable.Dispose() { }

		private static string[] moduleNames = new string[] { "ModuleAnimateGeneric", "ModuleAnimateGenericConsumer" };

		string[] ISwitchLights.ModuleNames => moduleNames;

		bool ISwitchLights.IsActive(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			return this.thisIsLight(tm);
		}

		bool ISwitchLights.IsSlowLight(PartModule pm, float thresholdInSecs)
		{
			TargetModule tm = (pm as TargetModule);
			return thresholdInSecs > (tm.animSpeed);
		}

		bool ISwitchLights.IsBeaconLight(PartModule pm) => false;
		bool ISwitchLights.IsNavigationLight(PartModule pm) => false;
		bool ISwitchLights.IsStrobeLight(PartModule pm) => false;
		bool ISwitchLights.IsUtilityLight(PartModule pm) => this.thisIsLight(pm as TargetModule);

		bool ISwitchLights.IsOn(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			Log.dbg("Part Module {0} is {1}", tm, tm.animSwitch);
			return this.thisIsLight(pm as TargetModule) && !tm.animSwitch;
		}

		void ISwitchLights.TurnOn(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			if (!this.thisIsLight(pm as TargetModule)) return;
			if (!(this as ISwitchLights).IsOn(pm))
				tm.Toggle();
			Log.dbg("Part Module {0} was turned on", tm);
		}

		void ISwitchLights.TurnOff(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			if (!this.thisIsLight(pm as TargetModule)) return;
			if ((this as ISwitchLights).IsOn(pm))
				tm.Toggle();
			Log.dbg("Part Module {0} was turned off", tm);
		}

		private bool thisIsLight(TargetModule tm)
		{
			//FIXME: Get rid of the Regex. This is a waste of CPU.
			//FIXME: Using the GUIName is dumb, it only works for English! I need a better way to identify these actions and events!
			return (Regex.IsMatch(tm.actionGUIName, "light", RegexOptions.IgnoreCase) || Regex.IsMatch(tm.startEventGUIName, "light", RegexOptions.IgnoreCase));
		}
	}
}
