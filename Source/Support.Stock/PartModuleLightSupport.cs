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
using TargetModule = global::ModuleLight;

namespace LASL.KSP.Support.SwitchLights.Stock
{
	public class Light : ISwitchLights
	{
		public Light() { }
		~Light() { }
		void System.IDisposable.Dispose() { }

		private static string[] moduleNames = new string[] { "ModuleLight"

															// Surface Mounted Lights, from ihsoft. These extends ModuleLight indirectly.
															, "ModuleStockLightColoredLens"
															, "ModuleMultiPointSurfaceLight"
															, "ModuleColoredLensLight"
														};
		string[] ISwitchLights.ModuleNames => moduleNames;

		bool ISwitchLights.IsActive(PartModule pm) => true;

		bool ISwitchLights.IsSlowLight(PartModule pm, float thresholdInSecs)
		{
			TargetModule tm = (pm as TargetModule);
			return thresholdInSecs > (tm.lightBrightenSpeed + tm.lightDimSpeed);
		}

		bool ISwitchLights.IsBeaconLight(PartModule pm) => false;
		bool ISwitchLights.IsNavigationLight(PartModule pm) => false;
		bool ISwitchLights.IsStrobeLight(PartModule pm) => false;
		bool ISwitchLights.IsUtilityLight(PartModule pm) => true;

		bool ISwitchLights.IsOn(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			Log.dbg("Part Module {0} is {1}", tm, tm.isOn);
			return tm.isOn;
		}

		void ISwitchLights.TurnOn(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			tm.LightsOn();
			Log.dbg("Part Module {0} was turned on", tm);
		}

		void ISwitchLights.TurnOff(PartModule pm)
		{
			TargetModule tm = (pm as TargetModule);
			tm.LightsOff();
			Log.dbg("Part Module {0} was turned off", tm);
		}
	}
}
