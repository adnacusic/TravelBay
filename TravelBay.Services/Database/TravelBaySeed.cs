using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TravelBay.Model.Constants;
using TravelBay.Model.Enums;

namespace TravelBay.Services.Database
{
    public partial class TravelBayDbContext : DbContext
    {
        private static readonly DateTime SeedBaseDate = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc);

        private void CreateSeed(ModelBuilder modelBuilder)
        {
            SeedCountries(modelBuilder);
            SeedCities(modelBuilder);
            SeedCategories(modelBuilder);
            SeedRoles(modelBuilder);
            SeedUsers(modelBuilder);
            SeedUserRoles(modelBuilder);
            SeedDestinations(modelBuilder);
            SeedDestinationImages(modelBuilder);
            SeedReviews(modelBuilder);
            SeedTripPlans(modelBuilder);
            SeedTripPlanItems(modelBuilder);
            SeedCollections(modelBuilder);
            SeedCollectionItems(modelBuilder);
            SeedSavedDestinations(modelBuilder);
            SeedUserPreferences(modelBuilder);
            SeedViewHistories(modelBuilder);
            SeedNotifications(modelBuilder);
            SeedNews(modelBuilder);
            SeedAuditLogs(modelBuilder);
        }

        private void SeedCountries(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(
                new { Id = 1, Name = "Bosna i Hercegovina" },
                new { Id = 2, Name = "Hrvatska" },
                new { Id = 3, Name = "Crna Gora" }
            );
        }

        private void SeedCities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new { Id = 1, Name = "Sarajevo", CountryId = 1 },
                new { Id = 2, Name = "Mostar", CountryId = 1 },
                new { Id = 3, Name = "Trebinje", CountryId = 1 },
                new { Id = 4, Name = "Neum", CountryId = 1 },
                new { Id = 5, Name = "Jajce", CountryId = 1 },
                new { Id = 6, Name = "Konjic", CountryId = 1 },
                new { Id = 7, Name = "Bihać", CountryId = 1 },
                new { Id = 8, Name = "Foča", CountryId = 1 },
                new { Id = 9, Name = "Ljubuški", CountryId = 1 },
                new { Id = 10, Name = "Blagaj", CountryId = 1 },
                new { Id = 11, Name = "Dubrovnik", CountryId = 2 },
                new { Id = 12, Name = "Split", CountryId = 2 },
                new { Id = 13, Name = "Kotor", CountryId = 3 },
                new { Id = 14, Name = "Budva", CountryId = 3 },
                new { Id = 15, Name = "Perast", CountryId = 3 }
            );
        }

        private void SeedCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new { Id = 1, Name = "Plaže", IconName = (string?)"beach", IsActive = true, CreatedAt = SeedBaseDate, UpdatedAt = (DateTime?)null },
                new { Id = 2, Name = "Planine", IconName = (string?)"mountain", IsActive = true, CreatedAt = SeedBaseDate, UpdatedAt = (DateTime?)null },
                new { Id = 3, Name = "Historija", IconName = (string?)"landmark", IsActive = true, CreatedAt = SeedBaseDate, UpdatedAt = (DateTime?)null },
                new { Id = 4, Name = "Hrana", IconName = (string?)"restaurant", IsActive = true, CreatedAt = SeedBaseDate, UpdatedAt = (DateTime?)null },
                new { Id = 5, Name = "Priroda", IconName = (string?)"nature", IsActive = true, CreatedAt = SeedBaseDate, UpdatedAt = (DateTime?)null }
            );
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new { Id = 1, Name = RoleNames.Admin, Description = "Administrator role with full permissions", IsActive = true, CreatedAt = SeedBaseDate },
                new { Id = 2, Name = RoleNames.User, Description = "Regular TravelBay user", IsActive = true, CreatedAt = SeedBaseDate }
            );
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            // Password for every seed user is "test". Hashes generated with the exact same
            // PBKDF2 (10000 iter, SHA256) algorithm as CryptoService, so login works out of the box.
            modelBuilder.Entity<User>().HasData(
                new
                {
                    Id = 1,
                    FirstName = "Desktop",
                    LastName = "Admin",
                    Email = "desktop@travelbay.local",
                    Username = "desktop",
                    PasswordHash = "WdE1iyTU37rDUJ6ZVWm+IVeaoZQ=",
                    PasswordSalt = "/vKjmkkerpD5Gm0uqqm6lQ==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                },
                new
                {
                    Id = 2,
                    FirstName = "Mobile",
                    LastName = "User",
                    Email = "mobile@travelbay.local",
                    Username = "mobile",
                    PasswordHash = "OH3vkoGdPG5KiEvWJuJu0AUTH7w=",
                    PasswordSalt = "7wPk2p3d/6zQEauDvWjVvQ==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                },
                new
                {
                    Id = 3,
                    FirstName = "Amina",
                    LastName = "Hodžić",
                    Email = "amina.hodzic@example.com",
                    Username = "amina",
                    PasswordHash = "+o3/xYezJVCJCsHOCG+7gBCSXsQ=",
                    PasswordSalt = "omcADDd4EkMqIHOQ+0KouQ==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                },
                new
                {
                    Id = 4,
                    FirstName = "Marko",
                    LastName = "Jurić",
                    Email = "marko.juric@example.com",
                    Username = "marko",
                    PasswordHash = "JZZOw5JAp986X1xigQcJz4D2qUI=",
                    PasswordSalt = "y740EVXQuhm68kDp+dp4KA==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                },
                new
                {
                    Id = 5,
                    FirstName = "Elena",
                    LastName = "Petrović",
                    Email = "elena.petrovic@example.com",
                    Username = "elena",
                    PasswordHash = "y/ML2lIEeQ2pXrL6J6JgjgsL+5Y=",
                    PasswordSalt = "/y/XeyiPaksiaR5Rqgp7GQ==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                },
                new
                {
                    Id = 6,
                    FirstName = "Ivan",
                    LastName = "Babić",
                    Email = "ivan.babic@example.com",
                    Username = "ivan",
                    PasswordHash = "hcUZJx0ppieMdpbFlffeg2Ztoos=",
                    PasswordSalt = "nY6lNc/fQh7jV/v1zo4q3Q==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                },
                new
                {
                    Id = 7,
                    FirstName = "Lara",
                    LastName = "Vuković",
                    Email = "lara.vukovic@example.com",
                    Username = "lara",
                    PasswordHash = "cahUhwK0z7Ll5sCJ4Yiiv571THI=",
                    PasswordSalt = "xFbTotl77sSQ3ejO04Ykrw==",
                    IsActive = true,
                    CreatedAt = SeedBaseDate,
                    LastLoginAt = (DateTime?)null,
                    PhoneNumber = (string?)null,
                    ProfileImageId = (int?)null
                }
            );
        }

        private void SeedUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasData(
                new { Id = 1, UserId = 1, RoleId = 1, DateAssigned = SeedBaseDate },
                new { Id = 2, UserId = 2, RoleId = 2, DateAssigned = SeedBaseDate },
                new { Id = 3, UserId = 3, RoleId = 2, DateAssigned = SeedBaseDate },
                new { Id = 4, UserId = 4, RoleId = 2, DateAssigned = SeedBaseDate },
                new { Id = 5, UserId = 5, RoleId = 2, DateAssigned = SeedBaseDate },
                new { Id = 6, UserId = 6, RoleId = 2, DateAssigned = SeedBaseDate },
                new { Id = 7, UserId = 7, RoleId = 2, DateAssigned = SeedBaseDate }
            );
        }

        /// <summary>(Id, Name, CategoryId, CityId, Description, HasKeywords)</summary>
        private static readonly (int Id, string Name, int CategoryId, int CityId, string Description, bool HasKeywords)[] DestinationSeedData = new[]
        {
            (1, "Stari Most", 3, 2, "Ikonični osmanski kameni most u Mostaru iz 16. vijeka, obnovljen 2004. godine i upisan na UNESCO-vu listu svjetske baštine.", true),
            (2, "Baščaršija", 3, 1, "Historijska trgovačka četvrt Sarajeva s brojnim zanatskim radnjama, džamijama i Sebiljem u srcu grada.", true),
            (3, "Počitelj", 3, 2, "Srednjovjekovno utvrđeno naselje iznad rijeke Neretve, poznato po kuli Hamam i kamenoj arhitekturi.", false),
            (4, "Tvrđava Jajce", 3, 5, "Srednjovjekovna tvrđava i posljednja prijestonica bosanskih kraljeva, smještena iznad vodopada Pliva.", false),
            (5, "Stari grad Trebinje", 3, 3, "Osmanska stara čaršija na obalama rijeke Trebišnjice, poznata po Arslanagića mostu u blizini.", true),
            (6, "Stari grad Dubrovnik", 3, 11, "Srednjovjekovni grad opasan zidinama, jedan od najbolje očuvanih utvrđenih gradova na Mediteranu.", true),
            (7, "Bjelašnica", 2, 1, "Planina iznad Sarajeva, olimpijsko skijalište iz 1984. godine s panoramskim pogledom na okolne vrhove.", false),
            (8, "Lukomir", 2, 1, "Najviše i najizolovanije stalno naseljeno selo u BiH, poznato po očuvanoj tradicionalnoj arhitekturi.", false),
            (9, "Orjen", 2, 3, "Najjužniji planinski masiv Dinarida s vrhovima iznad 1800m, blizu granice s Crnom Gorom.", false),
            (10, "Vrelo Bosne", 5, 1, "Izvorište rijeke Bosne u podnožju Igmana, uređeni park s alejama i jezercima — omiljeno izletište Sarajeva.", true),
            (11, "Vodopadi Kravica", 5, 9, "Tufasti vodopad na rijeci Trebižat visine oko 25m, okružen prirodnim amfiteatrom i jezerom pogodnim za kupanje.", true),
            (12, "Nacionalni park Una", 5, 7, "Nacionalni park oko rijeke Une poznate po bistroj vodi, brzacima i slapovima Štrbački buk.", false),
            (13, "Vrelo Bune (Blagaj)", 5, 10, "Jedan od najjačih krških izvora u Evropi, uz koji se nalazi derviška tekija iz 16. vijeka podno litice.", true),
            (14, "Nacionalni park Sutjeska", 5, 8, "Najstariji nacionalni park u BiH s prašumom Perućica i najvišim vrhom Maglić (2386m).", false),
            (15, "Boračko jezero", 5, 6, "Glacijalno jezero okruženo šumovitim planinama u blizini Konjica, pogodno za kupanje i rekreaciju.", false),
            (16, "Ćevabdžinice Baščaršije", 4, 1, "Tradicionalne sarajevske ćevabdžinice u srcu Baščaršije, poznate po ćevapima u somunu.", true),
            (17, "Vinski podrumi Trebinja", 4, 3, "Hercegovačka vinska regija oko Trebinja poznata po žilavki i vranacu.", false),
            (18, "Neum — morski specijaliteti", 4, 4, "Riblji restorani duž jedine bosanskohercegovačke obale, poznati po svježim plodovima mora.", false),
            (19, "Hercegovačka kulinarska tura", 4, 2, "Degustacija lokalnih specijaliteta Mostara i okoline — suhog mesa, sira i domaćeg vina.", false),
            (20, "Plaža Neum", 1, 4, "Glavna plaža jedinog bosanskohercegovačkog izlaza na more, s pogledom na Pelješki kanal.", true),
            (21, "Plaža Banje", 1, 11, "Gradska plaža Dubrovnika s pogledom na stare gradske zidine i ostrvo Lokrum.", true),
            (22, "Bačvice", 1, 12, "Najpoznatija splitska plaža u obliku školjke, poznata po igri 'picigin'.", false),
            (23, "Plaža Mogren", 1, 14, "Dvije uvale pješčano-šljunčane plaže ispod stare budvanske tvrđave.", false),
            (24, "Gospa od Škrpjela", 1, 15, "Umjetno ostrvo ispred Perasta s baroknom crkvom, nastalo nasipanjem kamena i potapanjem starih brodova.", true),
        };

        private void SeedDestinations(ModelBuilder modelBuilder)
        {
            var data = DestinationSeedData.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                CategoryId = d.CategoryId,
                CityId = d.CityId,
                Keywords = d.HasKeywords ? $"{d.Name}, {CategoryKeyword(d.CategoryId)}, izlet, odmor" : (string?)null,
                CreatedAt = SeedBaseDate.AddDays(d.Id),
                UpdatedAt = (DateTime?)null,
                IsDeleted = false
            }).ToArray();

            modelBuilder.Entity<Destination>().HasData(data);
        }

        private static string CategoryKeyword(int categoryId) => categoryId switch
        {
            1 => "plaža",
            2 => "planina",
            3 => "historija",
            4 => "gastronomija",
            5 => "priroda",
            _ => "putovanje"
        };

        private void SeedDestinationImages(ModelBuilder modelBuilder)
        {
            var withImages = DestinationSeedData.Where(d => d.Id != 4 && d.Id != 8 && d.Id != 9 && d.Id != 14 && d.Id != 17 && d.Id != 18 && d.Id != 23).ToArray();

            var data = withImages.Select((d, i) => new
            {
                Id = i + 1,
                DestinationId = d.Id,
                ImageUrl = $"https://picsum.photos/seed/travelbay-{d.Id}/800/600",
                Source = (string?)"Seed data",
                OrderIndex = 0,
                IsAiGenerated = false,
                CreatedAt = SeedBaseDate.AddDays(d.Id)
            }).ToArray();

            modelBuilder.Entity<DestinationImage>().HasData(data);
        }

        private void SeedReviews(ModelBuilder modelBuilder)
        {
            // (DestinationId, UserId, Rating, Comment, Status)
            var reviewSeeds = new (int DestinationId, int UserId, int Rating, string Comment, ReviewStatus Status)[]
            {
                (1, 3, 5, "Prelijep most, obavezno posjetiti u zoru dok nema gužve.", ReviewStatus.Approved),
                (1, 4, 4, "Impresivan prizor, ali dosta turista ljeti.", ReviewStatus.Approved),
                (2, 5, 5, "Baščaršija ima poseban šarm, hrana odlična.", ReviewStatus.Approved),
                (2, 6, 4, "Puno radnji sa suvenirima, preporučujem šetnju uveče.", ReviewStatus.Approved),
                (3, 7, 5, "Počitelj izgleda kao iz bajke, vrijedi penjanje do kule.", ReviewStatus.Approved),
                (5, 3, 4, "Mirna čaršija, lijep pogled na Trebišnjicu.", ReviewStatus.Approved),
                (6, 4, 5, "Zidine Dubrovnika su must-see, cijena ulaznice malo visoka.", ReviewStatus.Approved),
                (6, 5, 5, "Nevjerovatna arhitektura, ali ljeti pretrpano.", ReviewStatus.Pending),
                (7, 6, 4, "Odličan planinski zrak i staze za hiking.", ReviewStatus.Approved),
                (7, 7, 3, "Infrastruktura bi mogla biti bolja van sezone.", ReviewStatus.Approved),
                (8, 3, 5, "Lukomir je kao putovanje kroz vrijeme, nezaboravno.", ReviewStatus.Approved),
                (9, 4, 4, "Orjen je nepravedno zapostavljen, divni vidici.", ReviewStatus.Pending),
                (10, 5, 5, "Vrelo Bosne idealno za porodični izlet.", ReviewStatus.Approved),
                (10, 6, 4, "Lijepo uređeno, puno biciklista i trkača.", ReviewStatus.Approved),
                (11, 7, 5, "Kravica je spektakularna, vodopad oduzima dah.", ReviewStatus.Approved),
                (11, 3, 5, "Odlično za kupanje ljeti, dođite rano zbog gužve.", ReviewStatus.Approved),
                (12, 4, 4, "Rafting na Uni je top iskustvo.", ReviewStatus.Approved),
                (13, 5, 5, "Vrelo Bune i tekija su magično mjesto.", ReviewStatus.Approved),
                (13, 6, 2, "Previše turista odjednom, teško uživati u miru.", ReviewStatus.Rejected),
                (15, 7, 4, "Boračko jezero prelijepo za kupanje i piknik.", ReviewStatus.Approved),
                (16, 3, 5, "Najbolji ćevapi koje sam probala, apsolutno vrijedi.", ReviewStatus.Approved),
                (16, 4, 5, "Autentičan sarajevski doživljaj.", ReviewStatus.Approved),
                (18, 5, 3, "Hrana dobra ali skupo za ono što dobijete.", ReviewStatus.Pending),
                (19, 6, 4, "Vino i suho meso odlični, preporučujem turu.", ReviewStatus.Approved),
                (20, 7, 4, "Jedina naša plaža, uvijek lijepa za vikend.", ReviewStatus.Approved),
                (20, 3, 1, "Previše izgrađeno, izgubio šarm.", ReviewStatus.Rejected),
                (21, 4, 5, "Plaža Banje s pogledom na zidine — nezaboravno.", ReviewStatus.Approved),
                (21, 5, 4, "Skupo za ležaljke, ali pogled to opravdava.", ReviewStatus.Approved),
                (22, 6, 5, "Bačvice su institucija, obavezno probati picigin.", ReviewStatus.Approved),
                (24, 7, 5, "Gospa od Škrpjela je kao razglednica uživo.", ReviewStatus.Approved),
                (24, 3, 4, "Vožnja brodićem do ostrva je doživljaj za sebe.", ReviewStatus.Pending),
            };

            var data = reviewSeeds.Select((r, i) =>
            {
                var isModerated = r.Status != ReviewStatus.Pending;
                return new
                {
                    Id = i + 1,
                    DestinationId = r.DestinationId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = (string?)r.Comment,
                    Status = r.Status,
                    CreatedAt = SeedBaseDate.AddDays(20 + i),
                    ModeratedByUserId = isModerated ? (int?)1 : null,
                    ModeratedAt = isModerated ? (DateTime?)SeedBaseDate.AddDays(21 + i) : null,
                    ModerationReason = r.Status == ReviewStatus.Rejected ? "Neprimjeren ili netačan sadržaj recenzije." : (string?)null,
                    IsDeleted = false
                };
            }).ToArray();

            modelBuilder.Entity<Review>().HasData(data);
        }

        private void SeedTripPlans(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TripPlan>().HasData(
                new { Id = 1, UserId = 3, Name = "Vikend u Mostaru i Hercegovini", StartDate = SeedBaseDate.AddDays(40), EndDate = SeedBaseDate.AddDays(42), Status = TripPlanStatus.Draft, CreatedAt = SeedBaseDate.AddDays(30), UpdatedAt = (DateTime?)null, IsDeleted = false },
                new { Id = 2, UserId = 4, Name = "Ljeto na Jadranu", StartDate = SeedBaseDate.AddDays(60), EndDate = SeedBaseDate.AddDays(67), Status = TripPlanStatus.Active, CreatedAt = SeedBaseDate.AddDays(31), UpdatedAt = (DateTime?)SeedBaseDate.AddDays(35), IsDeleted = false },
                new { Id = 3, UserId = 5, Name = "Sedmica u planinama BiH", StartDate = SeedBaseDate.AddDays(10), EndDate = SeedBaseDate.AddDays(17), Status = TripPlanStatus.Completed, CreatedAt = SeedBaseDate.AddDays(5), UpdatedAt = (DateTime?)SeedBaseDate.AddDays(18), IsDeleted = false },
                new { Id = 4, UserId = 6, Name = "Istraživanje nacionalnih parkova", StartDate = SeedBaseDate.AddDays(25), EndDate = SeedBaseDate.AddDays(30), Status = TripPlanStatus.Cancelled, CreatedAt = SeedBaseDate.AddDays(20), UpdatedAt = (DateTime?)SeedBaseDate.AddDays(24), IsDeleted = false },
                new { Id = 5, UserId = 7, Name = "Kulturni obilazak regije", StartDate = SeedBaseDate.AddDays(50), EndDate = SeedBaseDate.AddDays(55), Status = TripPlanStatus.Active, CreatedAt = SeedBaseDate.AddDays(45), UpdatedAt = (DateTime?)null, IsDeleted = false },
                new { Id = 6, UserId = 3, Name = "Produženi vikend Sarajevo–Mostar", StartDate = SeedBaseDate.AddDays(80), EndDate = SeedBaseDate.AddDays(82), Status = TripPlanStatus.Draft, CreatedAt = SeedBaseDate.AddDays(70), UpdatedAt = (DateTime?)null, IsDeleted = false }
            );
        }

        private void SeedTripPlanItems(ModelBuilder modelBuilder)
        {
            // 3 destinations per trip plan, cycling through a varied destination pool.
            var destinationPool = new[] { 1, 2, 10, 7, 13, 11, 6, 21, 15, 8, 16, 24, 3, 12, 20, 5, 17, 9 };
            var items = new List<object>();
            int id = 1;
            for (int tripPlanId = 1; tripPlanId <= 6; tripPlanId++)
            {
                for (int day = 1; day <= 3; day++)
                {
                    var destinationId = destinationPool[((tripPlanId - 1) * 3 + (day - 1)) % destinationPool.Length];
                    items.Add(new
                    {
                        Id = id,
                        TripPlanId = tripPlanId,
                        DestinationId = destinationId,
                        DayNumber = day,
                        OrderIndex = day,
                        Notes = (string?)null
                    });
                    id++;
                }
            }

            modelBuilder.Entity<TripPlanItem>().HasData(items.ToArray());
        }

        private void SeedCollections(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collection>().HasData(
                new { Id = 1, UserId = 3, Name = "Omiljena mjesta", CreatedAt = SeedBaseDate.AddDays(10) },
                new { Id = 2, UserId = 4, Name = "Za sljedeće ljeto", CreatedAt = SeedBaseDate.AddDays(11) },
                new { Id = 3, UserId = 5, Name = "Planinski izleti", CreatedAt = SeedBaseDate.AddDays(12) },
                new { Id = 4, UserId = 6, Name = "Historijske znamenitosti", CreatedAt = SeedBaseDate.AddDays(13) },
                new { Id = 5, UserId = 7, Name = "Plaže za opuštanje", CreatedAt = SeedBaseDate.AddDays(14) },
                new { Id = 6, UserId = 2, Name = "Mobile test kolekcija", CreatedAt = SeedBaseDate.AddDays(15) }
            );
        }

        private void SeedCollectionItems(ModelBuilder modelBuilder)
        {
            var destinationPool = new[] { 2, 6, 13, 24, 7, 8, 15, 9, 1, 3, 4, 5, 20, 21, 22, 23, 10, 11 };
            var items = new List<object>();
            int id = 1;
            for (int collectionId = 1; collectionId <= 6; collectionId++)
            {
                for (int i = 0; i < 3; i++)
                {
                    var destinationId = destinationPool[((collectionId - 1) * 3 + i) % destinationPool.Length];
                    items.Add(new
                    {
                        Id = id,
                        CollectionId = collectionId,
                        DestinationId = destinationId,
                        AddedAt = SeedBaseDate.AddDays(10 + collectionId).AddHours(i)
                    });
                    id++;
                }
            }

            modelBuilder.Entity<CollectionItem>().HasData(items.ToArray());
        }

        private void SeedSavedDestinations(ModelBuilder modelBuilder)
        {
            // Each regular user (3-7) saves 4 distinct destinations — unique (UserId, DestinationId) by construction.
            var destinationPool = new[] { 1, 2, 10, 11, 13, 16, 20, 21, 24, 6, 7, 15, 8, 9, 3, 22, 5, 17, 12, 23 };
            var items = new List<object>();
            int id = 1;
            for (int userId = 3; userId <= 7; userId++)
            {
                for (int i = 0; i < 4; i++)
                {
                    var destinationId = destinationPool[((userId - 3) * 4 + i) % destinationPool.Length];
                    items.Add(new
                    {
                        Id = id,
                        UserId = userId,
                        DestinationId = destinationId,
                        SavedAt = SeedBaseDate.AddDays(5 + id)
                    });
                    id++;
                }
            }

            modelBuilder.Entity<SavedDestination>().HasData(items.ToArray());
        }

        private void SeedUserPreferences(ModelBuilder modelBuilder)
        {
            // Each regular user (3-7) prefers 2-3 distinct categories — unique (UserId, CategoryId) by construction.
            var prefsByUser = new Dictionary<int, int[]>
            {
                [3] = new[] { 3, 5 },
                [4] = new[] { 1, 4 },
                [5] = new[] { 2, 5, 3 },
                [6] = new[] { 3, 4 },
                [7] = new[] { 1, 2, 5 },
            };

            var items = new List<object>();
            int id = 1;
            foreach (var (userId, categoryIds) in prefsByUser)
            {
                foreach (var categoryId in categoryIds)
                {
                    items.Add(new { Id = id, UserId = userId, CategoryId = categoryId });
                    id++;
                }
            }

            modelBuilder.Entity<UserPreference>().HasData(items.ToArray());
        }

        private void SeedViewHistories(ModelBuilder modelBuilder)
        {
            // Realistic browsing history for the recommender — each regular user viewed several destinations.
            var destinationPool = new[] { 1, 2, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 21, 22, 24 };
            var items = new List<object>();
            int id = 1;
            for (int userId = 3; userId <= 7; userId++)
            {
                for (int i = 0; i < 7; i++)
                {
                    var destinationId = destinationPool[(userId * 7 + i) % destinationPool.Length];
                    items.Add(new
                    {
                        Id = id,
                        UserId = userId,
                        DestinationId = destinationId,
                        ViewedAt = SeedBaseDate.AddDays(1 + id * 0.5)
                    });
                    id++;
                }
            }

            modelBuilder.Entity<ViewHistory>().HasData(items.ToArray());
        }

        private void SeedNotifications(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>().HasData(
                new { Id = 1, UserId = 3, Title = "Recenzija odobrena", Message = "Vaša recenzija za Stari Most je odobrena.", Type = NotificationType.ReviewApproved, IsRead = true, CreatedAt = SeedBaseDate.AddDays(21) },
                new { Id = 2, UserId = 6, Title = "Recenzija odbijena", Message = "Vaša recenzija za Vrelo Bune nije odobrena.", Type = NotificationType.ReviewRejected, IsRead = false, CreatedAt = SeedBaseDate.AddDays(39) },
                new { Id = 3, UserId = 3, Title = "Recenzija odbijena", Message = "Vaša recenzija za Plažu Neum nije odobrena.", Type = NotificationType.ReviewRejected, IsRead = false, CreatedAt = SeedBaseDate.AddDays(45) },
                new { Id = 4, UserId = 4, Title = "Status plana putovanja promijenjen", Message = "Plan 'Ljeto na Jadranu' je sada aktivan.", Type = NotificationType.TripStatusChanged, IsRead = true, CreatedAt = SeedBaseDate.AddDays(35) },
                new { Id = 5, UserId = 5, Title = "Status plana putovanja promijenjen", Message = "Plan 'Sedmica u planinama BiH' je završen.", Type = NotificationType.TripStatusChanged, IsRead = true, CreatedAt = SeedBaseDate.AddDays(18) },
                new { Id = 6, UserId = 6, Title = "Status plana putovanja promijenjen", Message = "Plan 'Istraživanje nacionalnih parkova' je otkazan.", Type = NotificationType.TripStatusChanged, IsRead = false, CreatedAt = SeedBaseDate.AddDays(24) },
                new { Id = 7, UserId = 3, Title = "Nova vijest na TravelBayu", Message = "Pročitajte najnovije vijesti o turizmu u regiji.", Type = NotificationType.News, IsRead = false, CreatedAt = SeedBaseDate.AddDays(50) },
                new { Id = 8, UserId = 4, Title = "Nova vijest na TravelBayu", Message = "Pogledajte preporuke za jesenja putovanja.", Type = NotificationType.News, IsRead = false, CreatedAt = SeedBaseDate.AddDays(52) },
                new { Id = 9, UserId = 7, Title = "Dobrodošli na TravelBay", Message = "Hvala što ste se pridružili TravelBay zajednici.", Type = NotificationType.General, IsRead = true, CreatedAt = SeedBaseDate },
                new { Id = 10, UserId = 2, Title = "Dobrodošli na TravelBay", Message = "Hvala što ste se pridružili TravelBay zajednici.", Type = NotificationType.General, IsRead = true, CreatedAt = SeedBaseDate }
            );
        }

        private void SeedNews(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<News>().HasData(
                new { Id = 1, Title = "Sarajevo uvodi nove turističke rute", Content = "Grad Sarajevo pokreće tri nove pješačke ture kroz historijsko jezgro grada.", ImageUrl = "https://picsum.photos/seed/travelbay-news-1/800/400", PublishedAt = SeedBaseDate.AddDays(48), CreatedAt = SeedBaseDate.AddDays(48) },
                new { Id = 2, Title = "Rekordna sezona na Jadranu", Content = "Hrvatska i Crna Gora bilježe rekordan broj turista ove ljetne sezone.", ImageUrl = "https://picsum.photos/seed/travelbay-news-2/800/400", PublishedAt = SeedBaseDate.AddDays(49), CreatedAt = SeedBaseDate.AddDays(49) },
                new { Id = 3, Title = "Nacionalni parkovi otvaraju nove staze", Content = "NP Una i NP Sutjeska proširuju mrežu markiranih planinarskih staza.", ImageUrl = "https://picsum.photos/seed/travelbay-news-3/800/400", PublishedAt = SeedBaseDate.AddDays(50), CreatedAt = SeedBaseDate.AddDays(50) },
                new { Id = 4, Title = "Gastronomski festival u Mostaru", Content = "Hercegovačka kulinarska tura ugostit će regionalni gastro festival.", ImageUrl = "https://picsum.photos/seed/travelbay-news-4/800/400", PublishedAt = SeedBaseDate.AddDays(51), CreatedAt = SeedBaseDate.AddDays(51) },
                new { Id = 5, Title = "TravelBay predstavlja preporuke putovanja", Content = "Novi sistem preporuka pomaže korisnicima da pronađu destinacije po mjeri.", ImageUrl = "https://picsum.photos/seed/travelbay-news-5/800/400", PublishedAt = SeedBaseDate.AddDays(53), CreatedAt = SeedBaseDate.AddDays(53) }
            );
        }

        private void SeedAuditLogs(ModelBuilder modelBuilder)
        {
            // Mirrors the moderated (Approved/Rejected) reviews and the non-Draft trip plans seeded above.
            modelBuilder.Entity<AuditLog>().HasData(
                new { Id = 1, EntityName = nameof(Review), EntityId = 1, Action = "Approved", PerformedByUserId = (int?)1, PerformedAt = SeedBaseDate.AddDays(21), Details = (string?)"Review for destination 1 approved." },
                new { Id = 2, EntityName = nameof(Review), EntityId = 3, Action = "Approved", PerformedByUserId = (int?)1, PerformedAt = SeedBaseDate.AddDays(23), Details = (string?)"Review for destination 2 approved." },
                new { Id = 3, EntityName = nameof(Review), EntityId = 19, Action = "Rejected", PerformedByUserId = (int?)1, PerformedAt = SeedBaseDate.AddDays(39), Details = (string?)"Review for destination 13 rejected: neprimjeren sadržaj." },
                new { Id = 4, EntityName = nameof(Review), EntityId = 26, Action = "Rejected", PerformedByUserId = (int?)1, PerformedAt = SeedBaseDate.AddDays(45), Details = (string?)"Review for destination 20 rejected: neprimjeren sadržaj." },
                new { Id = 5, EntityName = nameof(TripPlan), EntityId = 2, Action = "StatusChanged:Active", PerformedByUserId = (int?)4, PerformedAt = SeedBaseDate.AddDays(35), Details = (string?)"Trip plan 'Ljeto na Jadranu' moved to Active." },
                new { Id = 6, EntityName = nameof(TripPlan), EntityId = 3, Action = "StatusChanged:Completed", PerformedByUserId = (int?)5, PerformedAt = SeedBaseDate.AddDays(18), Details = (string?)"Trip plan 'Sedmica u planinama BiH' moved to Completed." },
                new { Id = 7, EntityName = nameof(TripPlan), EntityId = 4, Action = "StatusChanged:Cancelled", PerformedByUserId = (int?)6, PerformedAt = SeedBaseDate.AddDays(24), Details = (string?)"Trip plan 'Istraživanje nacionalnih parkova' moved to Cancelled." },
                new { Id = 8, EntityName = nameof(TripPlan), EntityId = 5, Action = "StatusChanged:Active", PerformedByUserId = (int?)7, PerformedAt = SeedBaseDate.AddDays(45), Details = (string?)"Trip plan 'Kulturni obilazak regije' moved to Active." }
            );
        }
    }
}
