﻿using Streetcode.DAL.Entities.Base;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Entities.News
{
    public class News : IEntityId<int>
    { 
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string URL { get; set; } = string.Empty;

        public int? ImageId { get; set; }

        public Image? Image { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
