using Camaleao.Core.Enuns;
using Flunt.Notifications;
using Newtonsoft.Json;
using System.Linq;

namespace Camaleao.Core.Entities {
    public class Variable : Notifiable {

        private Variable() {
        }

        public Variable(string name, dynamic value, string type, dynamic initialize) {
            this.Name = name;
            this.Value = value;
            this.Type = type;
            this.Initialize = initialize;
            BuildVariable();
        }

        public string Name { get; private set; }
        public dynamic Initialize { get; private set; }
        public dynamic Value { get; private set; }
        public string Type { get; private set; }

        public void BuildVariable() {
            if (Initialize == null) {
                Value = GetTypeValue();
            }
            else {
                Value = MapperType();
            }
        }

        private string MapperType() {
            switch (Initialize?.GetType()?.FullName) {
                case "System.Int32":
                case "System.Int64": {
                        Type = "integer";
                        return $"{Initialize}";
                    }
                case "Newtonsoft.Json.Linq.JArray":
                case "Newtonsoft.Json.Linq.JObject": {
                        Type = "object";
                        return $"{JsonConvert.SerializeObject(Initialize)}";
                    }
                case "System.Double": {
                        Type = "double";
                        return $"{Initialize.ToString().Replace(',', '.')}";
                    }
                case "System.Boolean": {
                        Type = "bool";
                        return $"{Initialize.ToString().ToLower()}";
                    }
                default: {
                        Type = "text";
                        return $"'{Initialize}'";
                    }
            }
        }

        private string GetTypeValue() {
            switch (Type) {
                case "text":
                    return "''";
                case "integer":
                case "double":
                    return "0";
                case "bool":
                    return "true";
                case "array":
                    return "[]";
                default:
                    return "''";
            }
        }

        public bool IsValid() {

            if (string.IsNullOrWhiteSpace(Name)) {
                AddNotification("Variable", "[context.Variables] name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.Type)) {
                AddNotification("Variable", $"[context.Variables] type is required in { this.Name}.");
                return false;
            }

            if (!Enuns.VariableTypeEnum.GetValues().Contains(this.Type.ToLower())) {
                AddNotification("Variable", $"[context.Variables] type is required in { this.Name}.");
                return false;
            }
            return true;
        }


        public void SetValue(dynamic value) {
            if (this.Type?.ToLower() == VariableTypeEnum.Text && !string.IsNullOrWhiteSpace(value))
                Value = $"'{value}'";
            else if (this.Type?.ToLower() == VariableTypeEnum.Boolean && value != null)
                Value = value.ToLower();
            else
                Value = value;
        }
    }
}
