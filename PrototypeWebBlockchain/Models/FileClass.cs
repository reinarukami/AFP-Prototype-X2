using System.Web;
using System.ComponentModel.DataAnnotations;


namespace PrototypeWebBlockchain.Models
{
    public class FileClass
    {
        public HttpPostedFileBase image { get; set; }
    }
}