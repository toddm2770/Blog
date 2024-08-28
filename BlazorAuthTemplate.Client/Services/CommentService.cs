using BlazorAuthTemplate.client.Services.Interfaces;
using BlazorAuthTemplate.Client.Models;
using System.Net.Http.Json;

namespace BlazorAuthTemplate.Client.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;

        public CommentService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<CommentDTO> CreateCommentAsync(CommentDTO commentDTO)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/comments", commentDTO);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CommentDTO>()
                ?? throw new HttpRequestException("Invalid JSON revieved from server");
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/comments/{commentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
        {
            return await _httpClient.GetFromJsonAsync<CommentDTO>($"api/comments/{commentId}");
        }

        public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int postId)
        {
            var comments = await _httpClient.GetFromJsonAsync<IEnumerable<CommentDTO>>($"api/comments/{postId}") ?? [];
            return comments;
        }

        public async Task UpdateCommentAsync(CommentDTO commentDTO)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/comments/{commentDTO.Id}", commentDTO);
            response.EnsureSuccessStatusCode();
        }
    }
}
