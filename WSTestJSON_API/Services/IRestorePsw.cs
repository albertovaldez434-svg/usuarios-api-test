namespace WSTestJSON_API.Services
{
    public interface IRestorePsw
    {
        public Task SetNewPassword(string newPsw);
    }
}
