using Newtonsoft.Json;

namespace IDM.Models
{
    public enum TypeMensagem
    {
        Info,
        Erro
    }

    public class MensagemModel
    {
        public TypeMensagem Tipo {get; set;}

        public string Texto {get; set;}

        public MensagemModel(string mensagem, TypeMensagem tipo = TypeMensagem.Info)
        {
            this.Tipo = tipo;
            this.Texto = mensagem;
        }

        public static string Serializar(string mensagem, TypeMensagem tipo = TypeMensagem.Info)
        {
            var mensagemModel = new MensagemModel(mensagem, tipo);
            return JsonConvert.SerializeObject(mensagemModel);
        }

        public static MensagemModel Desserializar(string msgString)
        {
            return JsonConvert.DeserializeObject<MensagemModel>(msgString);
        }
    }
}