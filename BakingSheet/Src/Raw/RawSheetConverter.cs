﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cathei.BakingSheet.Raw
{
    public abstract class RawSheetConverter : RawSheetImporter, ISheetExporter
    {
        protected abstract Task<bool> SaveData();
        protected abstract IRawSheetExporterPage CreatePage(string sheetName);

        protected RawSheetConverter(TimeZoneInfo timeZoneInfo) : base(timeZoneInfo)
        {

        }

        public async Task<bool> Export(SheetConvertingContext context)
        {
            foreach (var prop in context.Container.GetSheetProperties())
            {
                var sheet = prop.GetValue(context.Container) as ISheet;
                if (sheet == null)
                    continue;

                var page = CreatePage(sheet.Name);

                context.SetTag(sheet.Name);
                page.Export(this, context, sheet);
            }

            var success = await SaveData();

            if (!success)
            {
                context.Logger.LogError($"Failed to save data");
                return false;
            }

            return true;
        }

        public virtual string ValueToString(SheetConvertingContext context, Type type, object value)
        {
            if (value == null)
                return null;

            if (value is ISheetReference)
            {
                var reference = value as ISheetReference;
                return ValueToString(context, reference.IdType, reference.Id);
            }

            if (value is DateTime)
            {
                var local = TimeZoneInfo.ConvertTimeFromUtc((DateTime)value, TimeZoneInfo);
                return local.ToString();
            }

            return value.ToString();
        }
    }
}
