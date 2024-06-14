using System.Transactions;
using Repositories.Interfaces;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Analytics;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Newss;
using Streetcode.DAL.Repositories.Interfaces.Partners;
using Streetcode.DAL.Repositories.Interfaces.Source;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Team;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Transactions;
using Streetcode.DAL.Repositories.Interfaces.Users;
using Streetcode.DAL.Repositories.Realizations.AdditionalContent;
using Streetcode.DAL.Repositories.Realizations.Analytics;
using Streetcode.DAL.Repositories.Realizations.Media;
using Streetcode.DAL.Repositories.Realizations.Media.Images;
using Streetcode.DAL.Repositories.Realizations.Newss;
using Streetcode.DAL.Repositories.Realizations.Partners;
using Streetcode.DAL.Repositories.Realizations.Source;
using Streetcode.DAL.Repositories.Realizations.Streetcode;
using Streetcode.DAL.Repositories.Realizations.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Realizations.Team;
using Streetcode.DAL.Repositories.Realizations.Timeline;
using Streetcode.DAL.Repositories.Realizations.Toponyms;
using Streetcode.DAL.Repositories.Realizations.Transactions;
using Streetcode.DAL.Repositories.Realizations.Users;

namespace Streetcode.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly StreetcodeDbContext _streetcodeDbContext;

    private IVideoRepository? _videoRepository;

    private IAudioRepository? _audioRepository;

    private IStreetcodeCoordinateRepository? _streetcodeCoordinateRepository;

    private IImageRepository? _imageRepository;

    private IImageDetailsRepository? _imageDetailsRepository;

    private IArtRepository? _artRepository;

    private IStreetcodeArtRepository? _streetcodeArtRepository;

    private IFactRepository? _factRepository;

    private IPartnersRepository? _partnersRepository;

    private ISourceCategoryRepository? _sourceCategoryRepository;

    private IStreetcodeCategoryContentRepository? _streetcodeCategoryContentRepository;

    private IRelatedFigureRepository? _relatedFigureRepository;

    private IRelatedTermRepository? _relatedTermRepository;

    private IStreetcodeRepository? _streetcodeRepository;

    private ISubtitleRepository? _subtitleRepository;

    private IStatisticRecordRepository? _statisticRecordRepository;

    private ITagRepository? _tagRepository;

    private ITermRepository? _termRepository;

    private ITeamRepository? _teamRepository;

    private IPositionRepository? _positionRepository;

    private ITextRepository? _textRepository;

    private ITimelineRepository? _timelineRepository;

    private IToponymRepository? _toponymRepository;

    private ITransactLinksRepository? _transactLinksRepository;

    private IHistoricalContextRepository? _historyContextRepository;

    private IPartnerSourceLinkRepository? _partnerSourceLinkRepository;

    private IUserRepository? _userRepository;

    private IStreetcodeTagIndexRepository? _streetcodeTagIndexRepository;

    private IPartnerStreetcodeRepository? _partnerStreetcodeRepository;

    private INewsRepository? _newsRepository;

    private ITeamLinkRepository? _teamLinkRepository;

    private ITeamPositionRepository? _teamPositionRepository;

    private IHistoricalContextTimelineRepository? _historicalContextTimelineRepository;

    private IStreetcodeToponymRepository? _streetcodeToponymRepository;

    private IStreetcodeImageRepository? _streetcodeImageRepository = null;

    public RepositoryWrapper(StreetcodeDbContext streetcodeDbContext)
    {
        _streetcodeDbContext = streetcodeDbContext;
    }

    public INewsRepository NewsRepository =>
          _newsRepository ??= new NewsRepository(_streetcodeDbContext);

    public IFactRepository FactRepository =>
          _factRepository ??= new FactRepository(_streetcodeDbContext);

    public IImageRepository ImageRepository =>
          _imageRepository ??= new ImageRepository(_streetcodeDbContext);

    public ITeamRepository TeamRepository =>
          _teamRepository ??= new TeamRepository(_streetcodeDbContext);

    public ITeamPositionRepository TeamPositionRepository =>
          _teamPositionRepository ??= new TeamPositionRepository(_streetcodeDbContext);

    public IAudioRepository AudioRepository =>
          _audioRepository ??= new AudioRepository(_streetcodeDbContext);

    public IStreetcodeCoordinateRepository StreetcodeCoordinateRepository =>
          _streetcodeCoordinateRepository ??= new StreetcodeCoordinateRepository(_streetcodeDbContext);

    public IVideoRepository VideoRepository =>
          _videoRepository ??= new VideoRepository(_streetcodeDbContext);

    public IArtRepository ArtRepository =>
          _artRepository ??= new ArtRepository(_streetcodeDbContext);

    public IStreetcodeArtRepository StreetcodeArtRepository =>
          _streetcodeArtRepository ??= new StreetcodeArtRepository(_streetcodeDbContext);

    public IPartnersRepository PartnersRepository =>
          _partnersRepository ??= new PartnersRepository(_streetcodeDbContext);

    public ISourceCategoryRepository SourceCategoryRepository =>
          _sourceCategoryRepository ??= new SourceCategoryRepository(_streetcodeDbContext);

    public IStreetcodeCategoryContentRepository StreetcodeCategoryContentRepository =>
          _streetcodeCategoryContentRepository ??= new StreetcodeCategoryContentRepository(_streetcodeDbContext);

    public IRelatedFigureRepository RelatedFigureRepository =>
          _relatedFigureRepository ??= new RelatedFigureRepository(_streetcodeDbContext);

    public IStreetcodeRepository StreetcodeRepository =>
          _streetcodeRepository ??= new StreetcodeRepository(_streetcodeDbContext);

    public ISubtitleRepository SubtitleRepository =>
          _subtitleRepository ??= new SubtitleRepository(_streetcodeDbContext);

    public IStatisticRecordRepository StatisticRecordRepository =>
          _statisticRecordRepository ??= new StatisticRecordsRepository(_streetcodeDbContext);

    public ITagRepository TagRepository =>
          _tagRepository ??= new TagRepository(_streetcodeDbContext);

    public ITermRepository TermRepository =>
          _termRepository ??= new TermRepository(_streetcodeDbContext);

    public ITextRepository TextRepository =>
          _textRepository ??= new TextRepository(_streetcodeDbContext);

    public ITimelineRepository TimelineRepository =>
          _timelineRepository ??= new TimelineRepository(_streetcodeDbContext);

    public IToponymRepository ToponymRepository =>
          _toponymRepository ??= new ToponymRepository(_streetcodeDbContext);

    public ITransactLinksRepository TransactLinksRepository =>
          _transactLinksRepository ??= new TransactLinksRepository(_streetcodeDbContext);

    public IHistoricalContextRepository HistoricalContextRepository =>
          _historyContextRepository ??= new HistoricalContextRepository(_streetcodeDbContext);

    public IPartnerSourceLinkRepository PartnerSourceLinkRepository =>
          _partnerSourceLinkRepository ??= new PartnersourceLinksRepository(_streetcodeDbContext);

    public IRelatedTermRepository RelatedTermRepository =>
          _relatedTermRepository ??= new RelatedTermRepository(_streetcodeDbContext);

    public IUserRepository UserRepository =>
          _userRepository ??= new UserRepository(_streetcodeDbContext);

    public IStreetcodeTagIndexRepository StreetcodeTagIndexRepository =>
        _streetcodeTagIndexRepository ??= new StreetcodeTagIndexRepository(_streetcodeDbContext);

    public IPartnerStreetcodeRepository PartnerStreetcodeRepository =>
        _partnerStreetcodeRepository ??= new PartnerStreetodeRepository(_streetcodeDbContext);

    public IPositionRepository PositionRepository =>
        _positionRepository ??= new PositionRepository(_streetcodeDbContext);

    public ITeamLinkRepository TeamLinkRepository =>
        _teamLinkRepository ??= new TeamLinkRepository(_streetcodeDbContext);

    public IImageDetailsRepository ImageDetailsRepository =>
        _imageDetailsRepository ??= new ImageDetailsRepository(_streetcodeDbContext);

    public IHistoricalContextTimelineRepository HistoricalContextTimelineRepository =>
        _historicalContextTimelineRepository ??= new HistoricalContextTimelineRepository(_streetcodeDbContext);

    public IStreetcodeToponymRepository StreetcodeToponymRepository =>
        _streetcodeToponymRepository ??= new StreetcodeToponymRepository(_streetcodeDbContext);

    // public IStreetcodeImageRepository StreetcodeImageRepository =>
    //    _streetcodeImageRepository ??= new StreetcodeImageRepository(_streetcodeDbContext);
    public IStreetcodeImageRepository StreetcodeImageRepository =>
        GetRepository(_streetcodeImageRepository as StreetcodeImageRepository);

    public T GetRepository<T>(T? repo)
     where T : IStreetcodeDbContextProvider, new()
    {
        if (repo is null)
        {
            repo = new T()
            {
                DbContext = _streetcodeDbContext
            };
        }

        return repo;
    }

    public int SaveChanges()
    {
        return _streetcodeDbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _streetcodeDbContext.SaveChangesAsync();
    }

    public TransactionScope BeginTransaction()
    {
        return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    }
}
