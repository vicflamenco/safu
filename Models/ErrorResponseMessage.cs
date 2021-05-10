namespace safuCHARTS.Models
{
    public class ErrorResponseMessage
    {
        public ErrorResponseMessage(CovalentResult covalentResult)
        {
            Error = covalentResult.ToString();
        }

        public string Error { get; set; }
    }
}
