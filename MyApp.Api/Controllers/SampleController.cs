using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BidFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        #region Public Endpoints - Herkes Erişebilir

        
        /// Genel ihale bilgileri - Kayıt olmadan görülebilir
        
        [HttpGet("public/tenders")]
        public IActionResult GetPublicTenders()
        {
            var publicTenders = new[]
            {
                new { Id = 1, Title = "Okul Binası İnşaatı", Deadline = "2025-09-15", Status = "Açık" },
                new { Id = 2, Title = "Park Düzenleme İşi", Deadline = "2025-10-01", Status = "Açık" }
            };

            return Ok(new
            {
                Message = "Genel ihaleler - Detaylar için üye olmanız gerekir",
                Tenders = publicTenders,
                Note = "Sadece başlık ve genel bilgiler görüntüleniyor"
            });
        }

        
        /// Sistem hakkında genel bilgi
        
        [HttpGet("public/info")]
        public IActionResult GetSystemInfo()
        {
            return Ok(new
            {
                SystemName = "BidFlow - İhale Takip Sistemi",
                Features = new[]
                {
                    "Güncel ihale listesi",
                    "Pro üyelik ile detaylı analiz",
                    "Favori ihaleler",
                    "E-posta bildirimleri"
                },
                Membership = new
                {
                    Free = "Temel ihale listesi",
                    Pro = "Detaylı analiz, özel raporlar, öncelikli destek"
                }
            });
        }

        #endregion

        #region User Endpoints - Giriş Yapmış Kullanıcılar

        
        /// Kullanıcı profil bilgileri
        
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var username = User.Identity?.Name;
            var isAdmin = bool.Parse(User.FindFirst("IsAdmin")?.Value ?? "false");
            var isPro = bool.Parse(User.FindFirst("IsPro")?.Value ?? "false");
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                UserId = userId,
                Username = username,
                Role = role,
                MembershipType = isPro ? "Pro Üye" : "Standart Üye",
                IsAdmin = isAdmin,
                AccountStatus = "Aktif",
                JoinDate = "2025-01-01",
                LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });
        }

        
        /// Standart ihaleler - Giriş yapmış kullanıcılar için
        
        [Authorize]
        [HttpGet("tenders")]
        public IActionResult GetTenders()
        {
            var isPro = bool.Parse(User.FindFirst("IsPro")?.Value ?? "false");

            var standardTenders = new[]
            {
                new {
                    Id = 1,
                    Title = "Okul Binası İnşaatı",
                    Deadline = "2025-09-15",
                    Status = "Açık",
                    Budget = "5.000.000 TL",
                    Location = "İstanbul",
                    Category = "İnşaat"
                },
                new {
                    Id = 2,
                    Title = "Park Düzenleme İşi",
                    Deadline = "2025-10-01",
                    Status = "Açık",
                    Budget = "750.000 TL",
                    Location = "Ankara",
                    Category = "Peyzaj"
                }
            };

            return Ok(new
            {
                Message = $"İhaleler - {(isPro ? "Pro" : "Standart")} üye görünümü",
                Tenders = standardTenders,
                TotalCount = standardTenders.Length,
                UserType = isPro ? "Pro" : "Standard",
                Note = isPro ? null : "Pro üye olarak daha detaylı bilgilere erişebilirsiniz"
            });
        }

        
        /// Kullanıcının favori ihaleleri
        
        [Authorize]
        [HttpGet("favorites")]
        public IActionResult GetFavorites()
        {
            var username = User.Identity?.Name;

            return Ok(new
            {
                Message = $"{username} kullanıcısının favori ihaleleri",
                Favorites = new[]
                {
                    new { Id = 1, Title = "Okul Binası İnşaatı", AddedDate = "2025-08-01" }
                },
                Count = 1
            });
        }

        #endregion

        #region Pro User Endpoints - Pro Üyeler

        
        /// Detaylı ihale analizi - Sadece Pro üyeler
        
        [Authorize(Roles = "Pro,Admin")]
        [HttpGet("tenders/detailed")]
        public IActionResult GetDetailedTenders()
        {
            var detailedTenders = new[]
            {
                new {
                    Id = 1,
                    Title = "Okul Binası İnşaatı",
                    Deadline = "2025-09-15",
                    Budget = "5.000.000 TL",
                    EstimatedParticipants = 15,
                    CompetitionLevel = "Yüksek",
                    ProfitMargin = "%12-18",
                    RiskLevel = "Orta",
                    PreviousWinner = "ABC İnşaat Ltd.",
                    RequiredExperience = "5+ yıl okul projesi",
                    TechnicalSpecs = "Betonarme yapı, 2000m² kapalı alan",
                    ContactPerson = "Mimar Ahmet Yılmaz - 0212-555-0123"
                }
            };

            return Ok(new
            {
                Message = "Pro üye detaylı ihale analizi",
                DetailedTenders = detailedTenders,
                AnalysisDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Note = "Bu detaylar sadece Pro üyeler tarafından görüntülenebilir"
            });
        }

        
        /// Özel raporlar - Sadece Pro üyeler
        
        [Authorize(Roles = "Pro,Admin")]
        [HttpGet("reports")]
        public IActionResult GetReports()
        {
            return Ok(new
            {
                Message = "Pro üye özel raporları",
                Reports = new[]
                {
                    new { Type = "Aylık Pazar Analizi", LastUpdate = "2025-08-01", Status = "Hazır" },
                    new { Type = "Rakip Firma Analizi", LastUpdate = "2025-07-28", Status = "Hazır" },
                    new { Type = "Sektörel Trend Raporu", LastUpdate = "2025-07-25", Status = "Hazır" }
                },
                NextReportDate = "2025-08-15"
            });
        }

        
        /// Gelişmiş arama filtreleri - Sadece Pro üyeler
        
        [Authorize(Roles = "Pro,Admin")]
        [HttpGet("search/advanced")]
        public IActionResult AdvancedSearch([FromQuery] string? category, [FromQuery] string? location, [FromQuery] int? minBudget)
        {
            return Ok(new
            {
                Message = "Pro üye gelişmiş arama",
                SearchCriteria = new { Category = category, Location = location, MinBudget = minBudget },
                AvailableFilters = new[]
                {
                    "Kategori bazlı filtreleme",
                    "Bütçe aralığı",
                    "Coğrafi konum",
                    "Teklif verme geçmişi",
                    "Kazanma oranı analizi"
                },
                Note = "Bu gelişmiş arama özellikleri sadece Pro üyeler için kullanılabilir"
            });
        }

        #endregion

        #region Admin Endpoints - Admin Kullanıcılar

        
        /// Tüm kullanıcıları listele - Sadece Admin
        
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public IActionResult GetAllUsers()
        {
            var users = new[]
            {
                new { Id = 1, Username = "john_doe", Email = "john@example.com", IsPro = false, IsAdmin = false, Status = "Aktif" },
                new { Id = 2, Username = "jane_pro", Email = "jane@example.com", IsPro = true, IsAdmin = false, Status = "Aktif" },
                new { Id = 3, Username = "admin_user", Email = "admin@example.com", IsPro = true, IsAdmin = true, Status = "Aktif" }
            };

            return Ok(new
            {
                Message = "Tüm kullanıcı listesi - Admin görünümü",
                Users = users,
                TotalUsers = users.Length,
                ActiveUsers = users.Count(u => u.Status == "Aktif"),
                ProUsers = users.Count(u => u.IsPro),
                AdminUsers = users.Count(u => u.IsAdmin)
            });
        }

        
        /// Kullanıcı güncelle - Sadece Admin
        
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/users/{userId}/update-membership")]
        public IActionResult UpdateUserMembership(int userId, [FromBody] UpdateMembershipRequest request)
        {
            // Gerçek projede database güncelleme işlemi yapılacak
            return Ok(new
            {
                Message = $"Kullanıcı {userId} üyelik durumu güncellendi",
                UpdatedBy = User.Identity?.Name,
                Changes = new
                {
                    UserId = userId,
                    NewIsPro = request.IsPro,
                    UpdateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                },
                Note = "Kullanıcı bir sonraki girişinde yeni yetkilerini görecek"
            });
        }

        
        /// Kullanıcı sil - Sadece Admin
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/users/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            // Gerçek projede database silme işlemi yapılacak
            return Ok(new
            {
                Message = $"Kullanıcı {userId} başarıyla silindi",
                DeletedBy = User.Identity?.Name,
                DeleteDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Note = "Bu işlem geri alınamaz"
            });
        }

        
        /// Sistem istatistikleri - Sadece Admin
        
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/statistics")]
        public IActionResult GetSystemStatistics()
        {
            return Ok(new
            {
                Message = "Sistem istatistikleri - Admin paneli",
                Statistics = new
                {
                    TotalUsers = 1250,
                    ProUsers = 180,
                    ActiveTenders = 45,
                    CompletedTenders = 230,
                    SystemUptime = "99.9%",
                    LastBackup = "2025-08-03 02:00",
                    StorageUsed = "2.4 GB"
                },
                RecentActivities = new[]
                {
                    "5 yeni kullanıcı kaydı",
                    "2 Pro üyelik yükseltmesi",
                    "12 yeni ihale eklendi"
                }
            });
        }

        #endregion

        #region Unauthorized Examples

        
        /// Bu endpoint'e sadece Pro kullanıcılar erişebilir - Test için
        
        [Authorize(Roles = "Pro")]
        [HttpGet("pro-only-test")]
        public IActionResult ProOnlyTest()
        {
            return Ok(new
            {
                Message = "Bu endpoint'e sadece Pro kullanıcılar erişebilir",
                AccessLevel = "Pro",
                Feature = "Premium İçerik Testi"
            });
        }

        #endregion
    }

    #region DTOs

    public class UpdateMembershipRequest
    {
        public bool IsPro { get; set; }
    }

    #endregion
}
