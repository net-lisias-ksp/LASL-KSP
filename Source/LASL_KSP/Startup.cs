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
using KSPe.Annotations;
using UnityEngine;

namespace LASL.KSP
{
	[KSPAddon (KSPAddon.Startup.Instantly, true)]
	internal class Startup : MonoBehaviour
	{
		[UsedImplicitly]
		private void Start ()
		{
			Log.force ("Version {0}", Version.Text);

			try {
				KSPe.Util.Installation.Check<Startup> ();
			} catch (KSPe.Util.InstallmentException e) {
				Log.error (e.ToShortMessage ());
				KSPe.Common.Dialogs.ShowStopperAlertBox.Show (e);
			}
#if false
			foreach (System.Reflection.Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
				Log.force("{0} -- {1}", asm.GetName().Name, asm.GetName().FullName);
			}
#endif
			try {
				using (KSPe.Util.SystemTools.Assembly.Loader a = new KSPe.Util.SystemTools.Assembly.Loader<Startup> ())
				{
					a.LoadAndStartup("LASL.KSP.Support.Stock");
					if (KSPe.Util.SystemTools.Assembly.Exists.ByName("AviationLights"))
						a.LoadAndStartup("LASL.KSP.Support.AviationLights");
					if (KSPe.Util.SystemTools.Assembly.Exists.ByName("KerbalElectric"))
						a.LoadAndStartup("LASL.KSP.Support.KerbalElectric");
					if (KSPe.Util.SystemTools.Assembly.Exists.ByName("WildBlueTools"))
						a.LoadAndStartup("LASL.KSP.Support.WildBlueTools");
				}
			} catch (KSPe.Util.InstallmentException e) {
				Log.error (e.ToShortMessage ());
				KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
			}
		}
	}
}
