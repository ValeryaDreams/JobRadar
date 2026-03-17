using Api.Models.DTO;

namespace Api.Services
{
        public interface ISuggestService
        {
                Task<SuggestResponseDto> SuggestAsync(string query);
        }
}
