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

        /// <summary>
        /// (Id, Name, CategoryId, CityId, Description, Keywords). The English keywords are the output of
        /// AIAgentKeywords (Groq) for the destinations that had none, copied here so a fresh database
        /// already has them; the others are the original seed keywords.
        /// </summary>
        private static readonly (int Id, string Name, int CategoryId, int CityId, string Description, string Keywords)[] DestinationSeedData = new[]
        {
            (1, "Stari Most", 3, 2, "Ikonični osmanski kameni most u Mostaru iz 16. vijeka, obnovljen 2004. godine i upisan na UNESCO-vu listu svjetske baštine.",
                "Stari Most, historija, izlet, odmor"),
            (2, "Baščaršija", 3, 1, "Historijska trgovačka četvrt Sarajeva s brojnim zanatskim radnjama, džamijama i Sebiljem u srcu grada.",
                "Baščaršija, historija, izlet, odmor"),
            (3, "Počitelj", 3, 2, "Srednjovjekovno utvrđeno naselje iznad rijeke Neretve, poznato po kuli Hamam i kamenoj arhitekturi.",
                "Počitelj Mostar Bosnia and Herzegovina, Medieval fortress, Neretva river, Stone architecture"),
            (4, "Tvrđava Jajce", 3, 5, "Srednjovjekovna tvrđava i posljednja prijestonica bosanskih kraljeva, smještena iznad vodopada Pliva.",
                "Tvrđava Jajce Jajce Bosnia and Herzegovina, medieval fortress, last Bosnian royal citadel, over Pliva waterfall"),
            (5, "Stari grad Trebinje", 3, 3, "Osmanska stara čaršija na obalama rijeke Trebišnjice, poznata po Arslanagića mostu u blizini.",
                "Stari grad Trebinje, historija, izlet, odmor"),
            (6, "Stari grad Dubrovnik", 3, 11, "Srednjovjekovni grad opasan zidinama, jedan od najbolje očuvanih utvrđenih gradova na Mediteranu.",
                "Stari grad Dubrovnik, historija, izlet, odmor"),
            (7, "Bjelašnica", 2, 1, "Planina iznad Sarajeva, olimpijsko skijalište iz 1984. godine s panoramskim pogledom na okolne vrhove.",
                "Bjelašnica Sarajevo Bosnia and Herzegovina, mountain, ski resort, Olympic 1984, panoramic view"),
            (8, "Lukomir", 2, 1, "Najviše i najizolovanije stalno naseljeno selo u BiH, poznato po očuvanoj tradicionalnoj arhitekturi.",
                "Lukomir Sarajevo Bosnia and Herzegovina, mountain village, traditional architecture, isolated settlement, highest village"),
            (9, "Orjen", 2, 3, "Najjužniji planinski masiv Dinarida s vrhovima iznad 1800m, blizu granice s Crnom Gorom.",
                "Orjen Trebinje Bosnia and Herzegovina, southern Dinaric mountain, mountain massif, border with Montenegro, peaks above 1800m"),
            (10, "Vrelo Bosne", 5, 1, "Izvorište rijeke Bosne u podnožju Igmana, uređeni park s alejama i jezercima — omiljeno izletište Sarajeva.",
                "Vrelo Bosne, priroda, izlet, odmor"),
            (11, "Vodopadi Kravica", 5, 9, "Tufasti vodopad na rijeci Trebižat visine oko 25m, okružen prirodnim amfiteatrom i jezerom pogodnim za kupanje.",
                "Vodopadi Kravica, priroda, izlet, odmor"),
            (12, "Nacionalni park Una", 5, 7, "Nacionalni park oko rijeke Une poznate po bistroj vodi, brzacima i slapovima Štrbački buk.",
                "Nacionalni park Una Bihać Bosnia and Herzegovina, river Una, clear water, rapids, Strbacki Buk waterfalls"),
            (13, "Vrelo Bune (Blagaj)", 5, 10, "Jedan od najjačih krških izvora u Evropi, uz koji se nalazi derviška tekija iz 16. vijeka podno litice.",
                "Vrelo Bune (Blagaj), priroda, izlet, odmor"),
            (14, "Nacionalni park Sutjeska", 5, 8, "Najstariji nacionalni park u BiH s prašumom Perućica i najvišim vrhom Maglić (2386m).",
                "Sutjeska National Park Foča Bosnia and Herzegovina, oldest national park, Perućica forest, Mount Maglić, 2386m peak"),
            (15, "Boračko jezero", 5, 6, "Glacijalno jezero okruženo šumovitim planinama u blizini Konjica, pogodno za kupanje i rekreaciju.",
                "Boračko jezero Konjic Bosnia and Herzegovina, glacial lake, forested mountains, swimming, recreation"),
            (16, "Ćevabdžinice Baščaršije", 4, 1, "Tradicionalne sarajevske ćevabdžinice u srcu Baščaršije, poznate po ćevapima u somunu.",
                "Ćevabdžinice Baščaršije, gastronomija, izlet, odmor"),
            (17, "Vinski podrumi Trebinja", 4, 3, "Hercegovačka vinska regija oko Trebinja poznata po žilavki i vranacu.",
                "Trebinje Bosnia and Herzegovina, wine cellar, herzegovinian wine, zlatna žilavka, vranac"),
            (18, "Neum — morski specijaliteti", 4, 4, "Riblji restorani duž jedine bosanskohercegovačke obale, poznati po svježim plodovima mora.",
                "Neum Bosnia and Herzegovina, seafood restaurants, fresh fish, coastal cuisine, Bosnian Adriatic"),
            (19, "Hercegovačka kulinarska tura", 4, 2, "Degustacija lokalnih specijaliteta Mostara i okoline — suhog mesa, sira i domaćeg vina.",
                "Herzegovinian culinary tour Mostar Bosnia and Herzegovina, dry meat, cheese, home wine"),
            (20, "Plaža Neum", 1, 4, "Glavna plaža jedinog bosanskohercegovačkog izlaza na more, s pogledom na Pelješki kanal.",
                "Plaža Neum, plaža, izlet, odmor"),
            (21, "Plaža Banje", 1, 11, "Gradska plaža Dubrovnika s pogledom na stare gradske zidine i ostrvo Lokrum.",
                "Plaža Banje, plaža, izlet, odmor"),
            (22, "Bačvice", 1, 12, "Najpoznatija splitska plaža u obliku školjke, poznata po igri 'picigin'.",
                "Bačvice Split Croatia, shaped like a shell, picigin game, popular beach"),
            (23, "Plaža Mogren", 1, 14, "Dvije uvale pješčano-šljunčane plaže ispod stare budvanske tvrđave.",
                "Plaža Mogren Budva Crna Gora, sand pebble beach, two coves, old fort view"),
            (24, "Gospa od Škrpjela", 1, 15, "Umjetno ostrvo ispred Perasta s baroknom crkvom, nastalo nasipanjem kamena i potapanjem starih brodova.",
                "Gospa od Škrpjela, plaža, izlet, odmor"),
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
                Keywords = (string?)d.Keywords,
                CreatedAt = SeedBaseDate.AddDays(d.Id),
                UpdatedAt = (DateTime?)null,
                IsDeleted = false
            }).ToArray();

            modelBuilder.Entity<Destination>().HasData(data);
        }

        /// <summary>
        /// (DestinationId, ImageUrl, Source): free Wikimedia Commons photos chosen by AIAgentSlike
        /// (hence IsAiGenerated), with license and author for attribution. Copied into the seed so a
        /// fresh database shows the right picture for every destination without running the agent.
        /// </summary>
        private static readonly (int DestinationId, string ImageUrl, string Source)[] DestinationImageSeedData = new[]
        {
            (1, "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d7/Mostar_Old_Town_Panorama_2007.jpg/1280px-Mostar_Old_Town_Panorama_2007.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Ramirez"),
            (2, "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/c5/Ba%C5%A1%C4%8Dar%C5%A1ija_2006.jpg/1280px-Ba%C5%A1%C4%8Dar%C5%A1ija_2006.jpg",
                "Wikimedia Commons · Public domain · en:User:Dark Apostrophe"),
            (3, "https://thumb.wikimedia.org/wikipedia/commons/thumb/7/7f/Pocitelj%2C_Capljina.JPG/1280px-Pocitelj%2C_Capljina.JPG",
                "Wikimedia Commons · CC BY 3.0 rs · korisnik srpske Wikipedije"),
            (4, "https://thumb.wikimedia.org/wikipedia/commons/thumb/2/2f/Jajce_Tvrdava.JPG/1280px-Jajce_Tvrdava.JPG",
                "Wikimedia Commons · CC BY-SA 4.0 · Dans"),
            (5, "https://thumb.wikimedia.org/wikipedia/commons/thumb/2/20/Trebinje_Stari_grad.jpg/1280px-Trebinje_Stari_grad.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · ER photography"),
            (6, "https://thumb.wikimedia.org/wikipedia/commons/thumb/a/a3/Dubrovnik%2C_stari_grad.jpg/1280px-Dubrovnik%2C_stari_grad.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Marko Jukić -Majkl"),
            (7, "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/8a/Bjela%C5%A1nica_1984.jpg/1280px-Bjela%C5%A1nica_1984.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Zavičajac"),
            (8, "https://upload.wikimedia.org/wikipedia/commons/1/1c/Lukomirske_udoli%2C_rozparcelovane_na_ovci_ohrady.jpg",
                "Wikimedia Commons · CC BY 3.0 · Martin Brož"),
            (9, "https://upload.wikimedia.org/wikipedia/commons/d/d3/Vyhled_z_Orjenu_na_S_-_Trebinje_v_BaH.jpg",
                "Wikimedia Commons · CC BY 3.0 · Martin Brož"),
            (10, "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/86/Vrelo_Bosne%2C_Sarajevo.jpg/1280px-Vrelo_Bosne%2C_Sarajevo.jpg",
                "Wikimedia Commons · CC BY 2.5 · Author's own photo"),
            (11, "https://thumb.wikimedia.org/wikipedia/commons/thumb/0/0d/Kravica_Waterfalls%2CBosnia_%26_Herzegovina.jpg/1280px-Kravica_Waterfalls%2CBosnia_%26_Herzegovina.jpg",
                "Wikimedia Commons · CC BY 2.0 · john mcsporran from Trossachs, Scotland"),
            (12, "https://thumb.wikimedia.org/wikipedia/commons/thumb/e/ee/Vuk%2C_Nacionalni_park_Una.jpg/1280px-Vuk%2C_Nacionalni_park_Una.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Nacionalni park Una"),
            (13, "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/42/Vrelo_Bune_i_tekija.jpg/1280px-Vrelo_Bune_i_tekija.jpg",
                "Wikimedia Commons · CC BY 2.0 · CJ"),
            (14, "https://upload.wikimedia.org/wikipedia/commons/e/ea/Peru%C4%87ica.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Minja Klarić"),
            (15, "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/82/Boracko_jezero.JPG/1280px-Boracko_jezero.JPG",
                "Wikimedia Commons · Public domain · Rochass"),
            (16, "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d7/Sarajevo%2C_panoramatick%C3%BD_pohled_na_Ba%C5%A1%C4%8Dar%C5%A1iji.jpg/1280px-Sarajevo%2C_panoramatick%C3%BD_pohled_na_Ba%C5%A1%C4%8Dar%C5%A1iji.jpg",
                "Wikimedia Commons · CC BY 3.0 · Aktron"), // približna, zamijeniti boljom
            (17, "https://thumb.wikimedia.org/wikipedia/commons/thumb/c/ca/Iglesia_Nova_Gracanica%2C_Trebinje%2C_Bosnia_y_Herzegovina%2C_2014-04-14%2C_DD_04.jpg/1280px-Iglesia_Nova_Gracanica%2C_Trebinje%2C_Bosnia_y_Herzegovina%2C_2014-04-14%2C_DD_04.jpg",
                "Wikimedia Commons · CC BY-SA 3.0 · Diego Delso"), // približna, zamijeniti boljom
            (18, "https://thumb.wikimedia.org/wikipedia/commons/thumb/1/12/Neum_%28BiH%29_-_general_view.jpg/1280px-Neum_%28BiH%29_-_general_view.jpg",
                "Wikimedia Commons · CC BY-SA 3.0 · Pudelek (Marcin Szala)"), // približna, zamijeniti boljom
            (19, "https://thumb.wikimedia.org/wikipedia/commons/thumb/3/33/Mostar_Stari_Most_BW_2024-10-01_11-58-11.jpg/1280px-Mostar_Stari_Most_BW_2024-10-01_11-58-11.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Berthold Werner"), // približna, zamijeniti boljom
            (20, "https://thumb.wikimedia.org/wikipedia/commons/thumb/5/5c/Zenit_Beach_in_Neum%2C_2024_2.jpg/1280px-Zenit_Beach_in_Neum%2C_2024_2.jpg",
                "Wikimedia Commons · CC0 · Bdx"),
            (21, "https://thumb.wikimedia.org/wikipedia/commons/thumb/7/76/Pla%C5%BEa_Banje_004.jpg/1280px-Pla%C5%BEa_Banje_004.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Vojtěch Dočkal"),
            (22, "https://upload.wikimedia.org/wikipedia/commons/d/dd/Bacvice.jpg",
                "Wikimedia Commons · CC BY-SA 1.0 · Airin"),
            (23, "https://thumb.wikimedia.org/wikipedia/commons/thumb/e/e3/01_Mogren_beach.jpg/1280px-01_Mogren_beach.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · BuhaM"),
            (24, "https://thumb.wikimedia.org/wikipedia/commons/thumb/6/65/Montenegro._Gospa_od_%C5%A0krpjela_15.jpg/1280px-Montenegro._Gospa_od_%C5%A0krpjela_15.jpg",
                "Wikimedia Commons · CC BY-SA 4.0 · Oktoober"),
        };

        private void SeedDestinationImages(ModelBuilder modelBuilder)
        {
            var data = DestinationImageSeedData.Select((image, i) => new
            {
                Id = i + 1,
                DestinationId = image.DestinationId,
                ImageUrl = image.ImageUrl,
                Source = (string?)image.Source,
                OrderIndex = 0,
                IsAiGenerated = true,
                AssetId = (int?)null,
                CreatedAt = SeedBaseDate.AddDays(image.DestinationId)
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
                (11, 2, 5, "Kravica nas je oduševila, voda je bila topla i čista.", ReviewStatus.Approved),
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
                new { Id = 6, UserId = 3, Name = "Produženi vikend Sarajevo–Mostar", StartDate = SeedBaseDate.AddDays(80), EndDate = SeedBaseDate.AddDays(82), Status = TripPlanStatus.Draft, CreatedAt = SeedBaseDate.AddDays(70), UpdatedAt = (DateTime?)null, IsDeleted = false },
                new { Id = 7, UserId = 2, Name = "Zima u Hercegovini", StartDate = SeedBaseDate.AddDays(330), EndDate = SeedBaseDate.AddDays(333), Status = TripPlanStatus.Active, CreatedAt = SeedBaseDate.AddDays(100), UpdatedAt = (DateTime?)SeedBaseDate.AddDays(101), IsDeleted = false },
                new { Id = 8, UserId = 2, Name = "Vikend na Uni", StartDate = SeedBaseDate.AddDays(120), EndDate = SeedBaseDate.AddDays(122), Status = TripPlanStatus.Completed, CreatedAt = SeedBaseDate.AddDays(90), UpdatedAt = (DateTime?)SeedBaseDate.AddDays(123), IsDeleted = false }
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

            // The mobile demo user's plans (7 active, 8 completed): (TripPlanId, DestinationId, DayNumber, OrderIndex, Notes)
            var mobilePlanItems = new (int TripPlanId, int DestinationId, int DayNumber, int OrderIndex, string? Notes)[]
            {
                (7, 1, 1, 1, "Šetnja Starim mostom u zoru."),
                (7, 3, 1, 2, null),
                (7, 13, 2, 1, "Ručak uz Vrelo Bune."),
                (8, 12, 1, 1, "Rafting na Uni."),
                (8, 15, 2, 1, null),
            };
            foreach (var item in mobilePlanItems)
            {
                items.Add(new
                {
                    Id = id,
                    TripPlanId = item.TripPlanId,
                    DestinationId = item.DestinationId,
                    DayNumber = item.DayNumber,
                    OrderIndex = item.OrderIndex,
                    Notes = item.Notes
                });
                id++;
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
                new { Id = 6, UserId = 2, Name = "Moja lista želja", CreatedAt = SeedBaseDate.AddDays(15) }
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

            foreach (var destinationId in new[] { 2, 11 })
            {
                items.Add(new { Id = id, UserId = 2, DestinationId = destinationId, SavedAt = SeedBaseDate.AddDays(100 + id) });
                id++;
            }

            modelBuilder.Entity<SavedDestination>().HasData(items.ToArray());
        }

        private void SeedUserPreferences(ModelBuilder modelBuilder)
        {
            // Each regular user prefers 2-3 distinct categories — unique (UserId, CategoryId) by construction.
            var prefsByUser = new Dictionary<int, int[]>
            {
                [3] = new[] { 3, 5 },
                [4] = new[] { 1, 4 },
                [5] = new[] { 2, 5, 3 },
                [6] = new[] { 3, 4 },
                [7] = new[] { 1, 2, 5 },
                [2] = new[] { 3, 5 },
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

            foreach (var destinationId in new[] { 1, 11, 13, 3, 12 })
            {
                items.Add(new { Id = id, UserId = 2, DestinationId = destinationId, ViewedAt = SeedBaseDate.AddDays(95 + id * 0.25) });
                id++;
            }

            modelBuilder.Entity<ViewHistory>().HasData(items.ToArray());
        }

        private void SeedNotifications(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>().HasData(
                new { Id = 1, UserId = 3, Title = "Recenzija odobrena", Message = "Vaša recenzija destinacije Stari Most je odobrena i sada je javno vidljiva.", Type = NotificationType.ReviewApproved, IsRead = true, CreatedAt = SeedBaseDate.AddDays(21) },
                new { Id = 2, UserId = 6, Title = "Recenzija odbijena", Message = "Vaša recenzija destinacije Vrelo Bune (Blagaj) nije objavljena. Razlog: Neprimjeren ili netačan sadržaj recenzije.", Type = NotificationType.ReviewRejected, IsRead = false, CreatedAt = SeedBaseDate.AddDays(39) },
                new { Id = 3, UserId = 3, Title = "Recenzija odbijena", Message = "Vaša recenzija destinacije Plaža Neum nije objavljena. Razlog: Neprimjeren ili netačan sadržaj recenzije.", Type = NotificationType.ReviewRejected, IsRead = false, CreatedAt = SeedBaseDate.AddDays(45) },
                new { Id = 4, UserId = 4, Title = "Status plana putovanja promijenjen", Message = "Plan 'Ljeto na Jadranu' je sada aktivan.", Type = NotificationType.TripStatusChanged, IsRead = true, CreatedAt = SeedBaseDate.AddDays(35) },
                new { Id = 5, UserId = 5, Title = "Status plana putovanja promijenjen", Message = "Plan 'Sedmica u planinama BiH' je sada završen.", Type = NotificationType.TripStatusChanged, IsRead = true, CreatedAt = SeedBaseDate.AddDays(18) },
                new { Id = 6, UserId = 6, Title = "Status plana putovanja promijenjen", Message = "Plan 'Istraživanje nacionalnih parkova' je sada otkazan.", Type = NotificationType.TripStatusChanged, IsRead = false, CreatedAt = SeedBaseDate.AddDays(24) },
                new { Id = 7, UserId = 3, Title = "Nova vijest na TravelBayu", Message = "Pročitajte najnovije vijesti o turizmu u regiji.", Type = NotificationType.News, IsRead = false, CreatedAt = SeedBaseDate.AddDays(50) },
                new { Id = 8, UserId = 4, Title = "Nova vijest na TravelBayu", Message = "Pogledajte preporuke za jesenja putovanja.", Type = NotificationType.News, IsRead = false, CreatedAt = SeedBaseDate.AddDays(52) },
                new { Id = 9, UserId = 7, Title = "Dobrodošli na TravelBay", Message = "Hvala što ste se pridružili TravelBay zajednici.", Type = NotificationType.General, IsRead = true, CreatedAt = SeedBaseDate },
                new { Id = 10, UserId = 2, Title = "Dobrodošli na TravelBay", Message = "Hvala što ste se pridružili TravelBay zajednici.", Type = NotificationType.General, IsRead = true, CreatedAt = SeedBaseDate },
                new { Id = 11, UserId = 2, Title = "Status plana putovanja promijenjen", Message = "Plan 'Vikend na Uni' je sada završen.", Type = NotificationType.TripStatusChanged, IsRead = true, CreatedAt = SeedBaseDate.AddDays(123) },
                new { Id = 12, UserId = 2, Title = "Recenzija odobrena", Message = "Vaša recenzija destinacije Vodopadi Kravica je odobrena i sada je javno vidljiva.", Type = NotificationType.ReviewApproved, IsRead = false, CreatedAt = SeedBaseDate.AddDays(52) }
            );
        }

        private void SeedNews(ModelBuilder modelBuilder)
        {
            // Free Wikimedia Commons photos (Sarajevo old town, Dalmatian coast, Maglić in NP Sutjeska,
            // stuffed peppers, Bay of Kotor) instead of placeholder images.
            modelBuilder.Entity<News>().HasData(
                new { Id = 1, Title = "Sarajevo uvodi nove turističke rute", Content = "Grad Sarajevo pokreće tri nove pješačke ture kroz historijsko jezgro grada.", ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/8/85/View_of_Sarajevo_Old_Town_at_Night..JPG/1280px-View_of_Sarajevo_Old_Town_at_Night..JPG", PublishedAt = SeedBaseDate.AddDays(48), CreatedAt = SeedBaseDate.AddDays(48) },
                new { Id = 2, Title = "Rekordna sezona na Jadranu", Content = "Hrvatska i Crna Gora bilježe rekordan broj turista ove ljetne sezone.", ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/4/47/01_Croatia_summer_vacation_destination_-_seaside_village_Dolac_between_Primosten_and_Bilo%2C_Dalmatia.jpg/1280px-01_Croatia_summer_vacation_destination_-_seaside_village_Dolac_between_Primosten_and_Bilo%2C_Dalmatia.jpg", PublishedAt = SeedBaseDate.AddDays(49), CreatedAt = SeedBaseDate.AddDays(49) },
                new { Id = 3, Title = "Nacionalni parkovi otvaraju nove staze", Content = "NP Una i NP Sutjeska proširuju mrežu markiranih planinarskih staza.", ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d5/Np_sutjeska_maglic.JPG/1280px-Np_sutjeska_maglic.JPG", PublishedAt = SeedBaseDate.AddDays(50), CreatedAt = SeedBaseDate.AddDays(50) },
                new { Id = 4, Title = "Gastronomski festival u Mostaru", Content = "Hercegovačka kulinarska tura ugostit će regionalni gastro festival.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/2/2b/Punjena_Paprika.JPG", PublishedAt = SeedBaseDate.AddDays(51), CreatedAt = SeedBaseDate.AddDays(51) },
                new { Id = 5, Title = "TravelBay predstavlja preporuke putovanja", Content = "Novi sistem preporuka pomaže korisnicima da pronađu destinacije po mjeri.", ImageUrl = "https://thumb.wikimedia.org/wikipedia/commons/thumb/d/d9/Kotor_and_Boka_kotorska_-_view_from_city_wall.jpg/1280px-Kotor_and_Boka_kotorska_-_view_from_city_wall.jpg", PublishedAt = SeedBaseDate.AddDays(53), CreatedAt = SeedBaseDate.AddDays(53) }
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
                new { Id = 8, EntityName = nameof(TripPlan), EntityId = 5, Action = "StatusChanged:Active", PerformedByUserId = (int?)7, PerformedAt = SeedBaseDate.AddDays(45), Details = (string?)"Trip plan 'Kulturni obilazak regije' moved to Active." },
                new { Id = 9, EntityName = nameof(Review), EntityId = 32, Action = "Approved", PerformedByUserId = (int?)1, PerformedAt = SeedBaseDate.AddDays(52), Details = (string?)"Review for destination 11 approved." },
                new { Id = 10, EntityName = nameof(TripPlan), EntityId = 7, Action = "StatusChanged:Active", PerformedByUserId = (int?)2, PerformedAt = SeedBaseDate.AddDays(101), Details = (string?)"Trip plan 'Zima u Hercegovini' moved to Active." },
                new { Id = 11, EntityName = nameof(TripPlan), EntityId = 8, Action = "StatusChanged:Completed", PerformedByUserId = (int?)2, PerformedAt = SeedBaseDate.AddDays(123), Details = (string?)"Trip plan 'Vikend na Uni' moved to Completed." }
            );
        }
    }
}
