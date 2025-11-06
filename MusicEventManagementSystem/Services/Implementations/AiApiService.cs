using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using MusicEventManagementSystem.Services;
using System;
using System.Threading.Tasks;
using MusicEventManagementSystem.Data; // THÊM
using MusicEventManagementSystem.Models; // THÊM
using System.Collections.Generic; // THÊM
using Microsoft.EntityFrameworkCore; // THÊM
using System.Linq; // THÊM


namespace MusicEventManagementSystem.Services.Implementations
{
    public class AiApiService : IAiService
    {
        private readonly OpenAIClient _client;
        private readonly string _model;
        private readonly MusicDbContext _context; // THÊM

        // SỬA CONSTRUCTOR: Thêm MusicDbContext
        public AiApiService(IConfiguration configuration, MusicDbContext context)
        {
            var apiKey = configuration["OpenAISettings:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key (OpenAISettings:ApiKey) không được cấu hình trong appsettings.json");
            }

            _client = new OpenAIClient(apiKey);
            _model = configuration["OpenAISettings:Model"] ?? "gpt-3.5-turbo";
            _context = context; // THÊM
        }

        // SỬA LẠI: Đã xóa "DateTime eventDate"
        public async Task<string> GenerateEventDescriptionAsync(string eventTitle, string eventType)
        {
            var systemPrompt = "You are a helpful assistant that writes compelling event descriptions.";

            // SỬA LẠI: Xóa tham chiếu đến eventDate
            var userPrompt = $"Generate an event description (about 100 words) for a {eventType} event titled '{eventTitle}'.";

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = _model,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userPrompt)
                },
                MaxTokens = 200,
                Temperature = 0.7f
            };

            try
            {
                Response<ChatCompletions> response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);
                string description = response.Value.Choices[0].Message.Content;
                return description?.Trim() ?? "Failed to generate description.";
            }
            catch (Exception ex)
            {
                return $"Error connecting to OpenAI: {ex.Message}";
            }
        }

        // ĐÃ XÓA HÀM GetChatReplyAsync BỊ TRÙNG (hàm giả lập)

        // Đây là hàm GetChatReplyAsync (thật)
        public async Task<string> GetChatReplyAsync(string userMessage)
        {
            var systemPrompt = "You are a helpful assistant for a music event website. " +
                               "Answer questions about music events. " +
                               "Be concise and friendly.";

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = _model,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userMessage)
                },
                MaxTokens = 150,
                Temperature = 0.5f
            };

            try
            {
                Response<ChatCompletions> response = await _client.GetChatCompletionsAsync(chatCompletionsOptions);
                string reply = response.Value.Choices[0].Message.Content;
                return reply?.Trim() ?? "Sorry, I couldn't understand that.";
            }
            catch (Exception ex)
            {
                return $"Sorry, AI service error: {ex.Message}";
            }
        }

        // THÊM PHƯƠNG THỨC MỚI NÀY
        public async Task<List<MusicEvent>> GetEventRecommendationsAsync(string userId)
        {
            // 1. Lấy sở thích (ví dụ, nếu bạn có model UserPreference)
            // (Phần này là giả lập, bạn có thể thay bằng logic lấy sở thích thật)
            var preferredGenre = "Pop"; 

            // 2. Lấy sự kiện sắp diễn ra
            var upcomingEvents = await _context.MusicEvents
                                               .Where(e => e.EventDate > DateTime.Now && e.IsPublished)
                                               .ToListAsync();

            // 3. Logic AI (giả lập): Lọc theo thể loại
            var recommendations = upcomingEvents
                .Where(e => !string.IsNullOrEmpty(e.Genre) && e.Genre.Equals(preferredGenre, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.EventDate)
                .Take(3)
                .ToList();

            // Nếu không có, lấy 3 sự kiện bất kỳ
            if (!recommendations.Any())
            {
                recommendations = upcomingEvents.OrderBy(e => e.EventDate).Take(3).ToList();
            }

            return recommendations;
        }
    }
}