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

namespace Streetcode.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly StreetcodeDbContext _streetcodeDbContext;

    private IVideoRepository? _videoRepository = null;

    private IAudioRepository? _audioRepository = null;

    private IStreetcodeCoordinateRepository? _streetcodeCoordinateRepository = null;

    private IImageRepository? _imageRepository = null;

    private IImageDetailsRepository? _imageDetailsRepository = null;

    private IArtRepository? _artRepository = null;

    private IStreetcodeArtRepository? _streetcodeArtRepository = null;

    private IFactRepository? _factRepository = null;

    private IPartnersRepository? _partnersRepository = null;

    private ISourceCategoryRepository? _sourceCategoryRepository = null;

    private IStreetcodeCategoryContentRepository? _streetcodeCategoryContentRepository = null;

    private IRelatedFigureRepository? _relatedFigureRepository = null;

    private IRelatedTermRepository? _relatedTermRepository = null;

    private IStreetcodeRepository? _streetcodeRepository = null;

    private ISubtitleRepository? _subtitleRepository = null;

    private IStatisticRecordRepository? _statisticRecordRepository = null;

    private ITagRepository? _tagRepository = null;

    private ITermRepository? _termRepository = null;

    private ITeamRepository? _teamRepository = null;

    private IPositionRepository? _positionRepository = null;

    private ITextRepository? _textRepository = null;

    private ITimelineRepository? _timelineRepository = null;

    private IToponymRepository? _toponymRepository = null;

    private ITransactLinksRepository? _transactLinksRepository = null;

    private IHistoricalContextRepository? _historyContextRepository = null;

    private IPartnerSourceLinkRepository? _partnerSourceLinkRepository = null;
   
    private IStreetcodeTagIndexRepository? _streetcodeTagIndexRepository = null;

    private IPartnerStreetcodeRepository? _partnerStreetcodeRepository = null;

    private INewsRepository? _newsRepository = null;

    private ITeamLinkRepository? _teamLinkRepository = null;

    private ITeamPositionRepository? _teamPositionRepository = null;

    private IHistoricalContextTimelineRepository? _historicalContextTimelineRepository = null;

    private IStreetcodeToponymRepository? _streetcodeToponymRepository = null;

    private IStreetcodeImageRepository? _streetcodeImageRepository = null;

    public RepositoryWrapper(StreetcodeDbContext streetcodeDbContext)
    {
        _streetcodeDbContext = streetcodeDbContext;
    }

    public INewsRepository NewsRepository =>
          GetRepository(_newsRepository as NewsRepository);

    public IFactRepository FactRepository =>
          GetRepository(_factRepository as FactRepository);

    public IImageRepository ImageRepository =>
          GetRepository(_imageRepository as ImageRepository);

    public ITeamRepository TeamRepository =>
          GetRepository(_teamRepository as TeamRepository);

    public ITeamPositionRepository TeamPositionRepository =>
          GetRepository(_teamPositionRepository as TeamPositionRepository);

    public IAudioRepository AudioRepository =>
          GetRepository(_audioRepository as AudioRepository);

    public IStreetcodeCoordinateRepository StreetcodeCoordinateRepository =>
          GetRepository(_streetcodeCoordinateRepository as StreetcodeCoordinateRepository);

    public IVideoRepository VideoRepository =>
          GetRepository(_videoRepository as VideoRepository);

    public IArtRepository ArtRepository =>
          GetRepository(_artRepository as ArtRepository);

    public IStreetcodeArtRepository StreetcodeArtRepository =>
          GetRepository(_streetcodeArtRepository as StreetcodeArtRepository);

    public IPartnersRepository PartnersRepository =>
          GetRepository(_partnersRepository as PartnersRepository);

    public ISourceCategoryRepository SourceCategoryRepository =>
          GetRepository(_sourceCategoryRepository as SourceCategoryRepository);

    public IStreetcodeCategoryContentRepository StreetcodeCategoryContentRepository =>
          GetRepository(_streetcodeCategoryContentRepository as StreetcodeCategoryContentRepository);

    public IRelatedFigureRepository RelatedFigureRepository =>
          GetRepository(_relatedFigureRepository as RelatedFigureRepository);

    public IStreetcodeRepository StreetcodeRepository =>
          GetRepository(_streetcodeRepository as StreetcodeRepository);

    public ISubtitleRepository SubtitleRepository =>
          GetRepository(_subtitleRepository as SubtitleRepository);

    public IStatisticRecordRepository StatisticRecordRepository =>
          GetRepository(_statisticRecordRepository as StatisticRecordsRepository);

    public ITagRepository TagRepository =>
          GetRepository(_tagRepository as TagRepository);

    public ITermRepository TermRepository =>
          GetRepository(_termRepository as TermRepository);

    public ITextRepository TextRepository =>
          GetRepository(_textRepository as TextRepository);

    public ITimelineRepository TimelineRepository =>
          GetRepository(_timelineRepository as TimelineRepository);

    public IToponymRepository ToponymRepository =>
          GetRepository(_toponymRepository as ToponymRepository);

    public ITransactLinksRepository TransactLinksRepository =>
          GetRepository(_transactLinksRepository as TransactLinksRepository);

    public IHistoricalContextRepository HistoricalContextRepository =>
          GetRepository(_historyContextRepository as HistoricalContextRepository);

    public IPartnerSourceLinkRepository PartnerSourceLinkRepository =>
          GetRepository(_partnerSourceLinkRepository as PartnersourceLinksRepository);

    public IRelatedTermRepository RelatedTermRepository =>
          GetRepository(_relatedTermRepository as RelatedTermRepository);
    
    public IStreetcodeTagIndexRepository StreetcodeTagIndexRepository =>
          GetRepository(_streetcodeTagIndexRepository as StreetcodeTagIndexRepository);

    public IPartnerStreetcodeRepository PartnerStreetcodeRepository =>
        GetRepository(_partnerStreetcodeRepository as PartnerStreetodeRepository);

    public IPositionRepository PositionRepository =>
        GetRepository(_positionRepository as PositionRepository);

    public ITeamLinkRepository TeamLinkRepository =>
          GetRepository(_teamLinkRepository as TeamLinkRepository);

    public IImageDetailsRepository ImageDetailsRepository =>
        GetRepository(_imageDetailsRepository as ImageDetailsRepository);

    public IHistoricalContextTimelineRepository HistoricalContextTimelineRepository =>
        GetRepository(_historicalContextTimelineRepository as HistoricalContextTimelineRepository);

    public IStreetcodeToponymRepository StreetcodeToponymRepository =>
        GetRepository(_streetcodeToponymRepository as StreetcodeToponymRepository);

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