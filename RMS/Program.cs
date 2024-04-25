using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models;

namespace RMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            //builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();


            var app = builder.Build();

            // Seeding
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    IdentityResult roleResult = roleManager.CreateAsync(new("Admin")).Result;
                }

                if (!roleManager.RoleExistsAsync("Employee").Result)
                {
                    IdentityResult roleResult = roleManager.CreateAsync(new("Employee")).Result;
                }

                if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
                {
                    AppUser user = new()
                    {
                        Name = "Admin",
                        PhoneNumber = "0123456789",
                        PhoneNumberConfirmed = true,
                        Address = "Gaming Street",
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Abc123##").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                if (userManager.FindByEmailAsync("emp@gmail.com").Result == null)
                {
                    AppUser user = new()
                    {
                        Name = "Employee",
                        PhoneNumber = "0123456789",
                        PhoneNumberConfirmed = true,
                        Address = "Gaming Street",
                        UserName = "emp",
                        Email = "emp@gmail.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Abc123##").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Employee").Wait();
                    }
                }

                if (userManager.FindByEmailAsync("user@gmail.com").Result == null)
                {
                    AppUser user = new AppUser
                    {
                        Name = "User",
                        PhoneNumber = "0123456789",
                        PhoneNumberConfirmed = true,
                        Address = "Gaming Street",
                        UserName = "user",
                        Email = "user@gmail.com",
                        EmailConfirmed = true
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Abc123##").Result;
                }

                // categories
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name = "Burgers", Description = "Delicious beef or veggie patties served in buns with various toppings and condiments." },
                        new Category { Name = "Pizza", Description = "Traditional Italian dish consisting of a round, flat base of dough topped with tomato sauce, cheese, and various toppings." },
                        new Category { Name = "Shawarma", Description = "Middle Eastern dish made from thinly sliced meat (often chicken, lamb, or beef) wrapped in a flatbread with vegetables and sauces." },
                        new Category { Name = "Sandwiches", Description = "Versatile dish consisting of various ingredients such as meat, cheese, vegetables, and condiments placed between slices of bread." },
                        new Category { Name = "Wraps", Description = "Similar to sandwiches but with ingredients wrapped in a tortilla or flatbread instead of between slices of bread." }
                    );

                    context.SaveChanges();
                }

                // Menu Items
                if (!context.MenuItems.Any())
                {
                    // Get category IDs from the database
                    var burgersCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Burgers").Id;
                    var pizzaCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Pizza").Id;
                    var shawarmaCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Shawarma").Id;
                    var sandwichesCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Sandwiches").Id;
                    var wrapsCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Wraps").Id;

                    context.MenuItems.AddRange(
                        new MenuItem
                        {
                            Name = "Cheese Burger",
                            Price = 10,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.thecookierookie.com/wp-content/uploads/2023/04/featured-stovetop-burgers-recipe.jpg",
                            CategoryId = burgersCategoryId
                        },
                        new MenuItem
                        {
                            Name = "Pepperoni Pizza",
                            Price = 15,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.foodandwine.com/thmb/Wd4lBRZz3X_8qBr69UOu2m7I2iw=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/classic-cheese-pizza-FT-RECIPE0422-31a2c938fc2546c9a07b7011658cfd05.jpg",
                            CategoryId = pizzaCategoryId
                        },
                        new MenuItem
                        {
                            Name = "Chicken Shawarma",
                            Price = 12,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQyom7gjhxyPBoqnmWExDaYQcsNKk6pfh0_W4NY0eEFJQ&s",
                            CategoryId = shawarmaCategoryId
                        },
                        new MenuItem
                        {
                            Name = "Club Sandwich",
                            Price = 8,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://static.toiimg.com/thumb/83740315.cms?imgsize=361903&width=800&height=800",
                            CategoryId = sandwichesCategoryId
                        },
                        new MenuItem
                        {
                            Name = "Chicken Wrap",
                            Price = 9,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.licious.in/blog/wp-content/uploads/2020/12/Chicken-Wrap.jpg",
                            CategoryId = wrapsCategoryId
                        },
                        new MenuItem
                        {
                            Name = "Cheese Burger 2",
                            Price = 10,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.thecookierookie.com/wp-content/uploads/2023/04/featured-stovetop-burgers-recipe.jpg",
                            CategoryId = burgersCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Pepperoni Pizza 2",
                            Price = 15,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.foodandwine.com/thmb/Wd4lBRZz3X_8qBr69UOu2m7I2iw=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/classic-cheese-pizza-FT-RECIPE0422-31a2c938fc2546c9a07b7011658cfd05.jpg",
                            CategoryId = pizzaCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Chicken Shawarma 2",
                            Price = 12,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQyom7gjhxyPBoqnmWExDaYQcsNKk6pfh0_W4NY0eEFJQ&s",
                            CategoryId = shawarmaCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Club Sandwich 2",
                            Price = 8,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://static.toiimg.com/thumb/83740315.cms?imgsize=361903&width=800&height=800",
                            CategoryId = sandwichesCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Chicken Wrap 2",
                            Price = 9,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.licious.in/blog/wp-content/uploads/2020/12/Chicken-Wrap.jpg",
                            CategoryId = wrapsCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Cheese Burger 3",
                            Price = 10,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.thecookierookie.com/wp-content/uploads/2023/04/featured-stovetop-burgers-recipe.jpg",
                            CategoryId = burgersCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Pepperoni Pizza 3",
                            Price = 15,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.foodandwine.com/thmb/Wd4lBRZz3X_8qBr69UOu2m7I2iw=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/classic-cheese-pizza-FT-RECIPE0422-31a2c938fc2546c9a07b7011658cfd05.jpg",
                            CategoryId = pizzaCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Chicken Shawarma 3",
                            Price = 12,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQyom7gjhxyPBoqnmWExDaYQcsNKk6pfh0_W4NY0eEFJQ&s",
                            CategoryId = shawarmaCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Club Sandwich 3",
                            Price = 8,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://static.toiimg.com/thumb/83740315.cms?imgsize=361903&width=800&height=800",
                            CategoryId = sandwichesCategoryId // Using the retrieved category ID
                        },
                        new MenuItem
                        {
                            Name = "Chicken Wrap 3",
                            Price = 9,
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                            ImageURL = "https://www.licious.in/blog/wp-content/uploads/2020/12/Chicken-Wrap.jpg",
                            CategoryId = wrapsCategoryId // Using the retrieved category ID
                        }
                        );

                        context.SaveChanges();
                }


            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapAreaControllerRoute("adminArea", "Admin", "Admin/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}