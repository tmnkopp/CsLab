using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CyberScope.Automator
{
    public interface IConnect
    {
        void Connect(SessionContext context);
    } 
}