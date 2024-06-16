namespace Streetcode.XUnitTest.MediatRTests.Sources;

using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.Sources;
using BLL.Interfaces.BlobStorage;
using BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

public class GetCategoriesByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly GetCategoriesByStreetcodeIdHandler _handler;
    private readonly List<SourceLinkCategory> _listSourceCategories;
    private readonly List<SourceLinkCategoryDTO> _listSourceCategoryDto;

    public GetCategoriesByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _mapperMock = new Mock<IMapper>();
        _blobServiceMock = new Mock<IBlobService>();
        Mock<ILoggerService> loggerMock = new();
        _handler = new GetCategoriesByStreetcodeIdHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _blobServiceMock.Object, loggerMock.Object);
        _listSourceCategories = new List<SourceLinkCategory>
        {
            new ()
            {
                Id = 1,
                Title = "Test1",
                ImageId = 1,
                Image = new () { BlobName = "blob1", Base64 = string.Empty },
                Streetcodes = new () { new StreetcodeContent() { Id = 1 }, },
            },
        };

        _listSourceCategoryDto = new List<SourceLinkCategoryDTO>
        {
            new ()
            {
                Id = 1,
                Title = "Test1",
                ImageId = 1,
                Image = new ImageDTO { BlobName = "blob1", Base64 = string.Empty },
            },
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCategoriesAreNull()
    {
        // Arrange
        var request = new GetCategoriesByStreetcodeIdQuery(1);
        IError error = new Error($"Cant find any source category with the streetcode id {request.StreetcodeId}");

        _repositoryWrapperMock.Setup(repo => repo.SourceCategoryRepository.GetAllAsync(
            It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
            It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()))
            .ReturnsAsync((IEnumerable<SourceLinkCategory>)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Errors?.Exists(e => e.Message == error.Message));
    }

    [Fact]
    public async Task Handle_ShouldReturnIEnumarableCategoryDTOs_WhenCategoriesExist()
    {
        // Arrange
        string base64 = "base64-encoded-string-1";

        _repositoryWrapperMock.Setup(repo => repo.SourceCategoryRepository.GetAllAsync(
             It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
             It.IsAny<Func<IQueryable<SourceLinkCategory>, IIncludableQueryable<SourceLinkCategory, object>>>()))
            .ReturnsAsync(_listSourceCategories);

        _blobServiceMock.Setup(blobService => blobService.FindFileInStorageAsBase64("blob1")).Returns(base64);

        _mapperMock.Setup(mapper => mapper.Map<IEnumerable<SourceLinkCategoryDTO>>(_listSourceCategories))
            .Returns(_listSourceCategoryDto);

        // Act
        await _handler.Handle(new GetCategoriesByStreetcodeIdQuery(1), CancellationToken.None);

        // Assert
        Assert.Equal(_listSourceCategoryDto[0].Image?.Base64, base64);
    }
}