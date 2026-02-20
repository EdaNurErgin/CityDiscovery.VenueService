using CityDiscovery.Venues.Application.Interfaces.Repositories;
using CityDiscovery.Venues.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CityDiscovery.Venues.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // 1. Yetki Kontrolü
        if (request.UserRole != "Admin")
        {
            _logger.LogWarning("Unauthorized category creation attempt. User: {UserId}", request.UserId);
            throw new UnauthorizedAccessException("Bu işlemi sadece sistem yöneticisi (Admin) yapabilir.");
        }

        // 2. İsim bazlı benzersizlik kontrolü
        var exists = await _categoryRepository.ExistsAsync(request.Name, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"'{request.Name}' adında bir kategori sistemde zaten mevcut.");
        }

        // 3. Slug (URL formatı) oluşturma (Örn: "Gece Kulübü" -> "gece-kulübü")
        var slug = request.Name.ToLowerInvariant().Replace(" ", "-");

        // 4. Domain entity'nin kendi Create metodu ile oluşturulması (Hata veren yerin çözümü)
        var category = Category.Create(request.Name, slug, request.IconUrl);

        // 5. Veritabanına kayıt işlemi
        await _categoryRepository.AddAsync(category, cancellationToken);

        _logger.LogInformation("Category '{CategoryName}' created successfully by Admin {UserId}", category.Name, request.UserId);

        // Not: AddAsync metodu içinde SaveChanges çalışıyorsa CategoryId dolacaktır.
        return category.CategoryId;
    }
}