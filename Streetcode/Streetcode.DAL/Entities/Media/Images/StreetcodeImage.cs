﻿using System.ComponentModel.DataAnnotations;
using Streetcode.DAL.Entities.Base;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Media.Images
{
    public class StreetcodeImage : IEntity
    {
        public int StreetcodeId { get; set; }

        public int ImageId { get; set; }

        public Image? Image { get; set; }

        public StreetcodeContent? Streetcode { get; set; }
    }
}
