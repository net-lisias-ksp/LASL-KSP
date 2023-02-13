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

namespace LASL.KSP.SwitchLight.AviationLights
{
	[KSPAddon (KSPAddon.Startup.Instantly, true)]
	internal class Startup : MonoBehaviour
	{
		[UsedImplicitly]
		private void Start ()
		{
			Log.force ("Aviation Lights Support for LASL.KSP Version {0}", Version.Text);
		}
	}
}
