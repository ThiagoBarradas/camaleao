using System.ComponentModel.DataAnnotations;

namespace Camaleao.Application.TemplateAgg.Models {
    public class RouteModel {
        [Required(ErrorMessageResourceName = "VersionRequired")]
        public string Version { get; set; }
        [Required(ErrorMessageResourceName = "NameRequired")]
        public string Name { get; set; }
        [Required(ErrorMessageResourceName = "MethodRequired")]
        public string Method { get; set; }
    }
}
