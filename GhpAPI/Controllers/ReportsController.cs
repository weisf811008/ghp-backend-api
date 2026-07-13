using GhpAPI.Data;
using GhpAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Tags("報表管理")]
    [Authorize(Roles = "學校管理員,巡檢人員")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        private int GetSchoolId()
        {
            return int.Parse(User.FindFirst("schoolId")!.Value);
        }

        // GET api/reports/daily?start=2026-01-01&end=2026-01-31
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport(DateTime start, DateTime end)
        {
            var schoolId = GetSchoolId();

            // 查詢指定日期範圍內的巡檢狀態
            var rows = await (
                from i in _db.Inspections
                join ind in _db.InspectionDetails on i.Id equals ind.InspectionId
                join item in _db.Items on ind.ItemId equals item.Id
                join c in _db.Categories on item.CategoryId equals c.Id
                where i.SchoolId == schoolId
                    && i.Date >= start
                    && i.Date <= end
                select new
                {
                    item.Id,
                    item.No,
                    item.Item,
                    Category = c.Category,
                    Date = i.Date,
                    ind.Status,
                    ind.Remarks,
                }
            ).ToListAsync();

            // 組合 rows（每個 item 一筆，日期當 key）
            var rowMap = new Dictionary<int, Dictionary<string, object?>>();
            var abnormalRows = new List<AbnormalRowDto>();

            foreach (var row in rows)
            {
                var date = row.Date.ToString("yyyy-MM-dd");

                if (!rowMap.ContainsKey(row.Id))
                {
                    rowMap[row.Id] = new Dictionary<string, object?>
                    {
                        { "itemId", row.Id },
                        { "no", row.No },
                        { "item", row.Item },
                        { "category", row.Category },
                    };
                }

                if (!rowMap[row.Id].ContainsKey(date))
                {
                    rowMap[row.Id][date] = row.Status;

                    if (row.Status != "pass")
                    {
                        abnormalRows.Add(new AbnormalRowDto
                        {
                            Date = date,
                            No = row.No,
                            Category = row.Category,
                            Item = row.Item,
                            Status = row.Status,
                            Remarks = row.Remarks,
                        });
                    }
                }
            }

            return Ok(new DailyReportDto
            {
                Rows = rowMap.Values.ToList(),
                AbnormalRows = abnormalRows,
            });
        }

        //GET api/reports/ghp
        [HttpGet("ghp")]

        public async Task<IActionResult> getGhpReport(DateTime start, DateTime end)
        {
            var schoolId = GetSchoolId();
            var rows = await (
                   from i in _db.Inspections
                   join ind in _db.InspectionDetails on i.Id equals ind.InspectionId
                   join item in _db.Items on ind.ItemId equals item.Id
                   join ir in _db.ItemRegulations on item.Id equals ir.ItemId
                   join r in _db.Regulations on ir.RegulationId equals r.Id
                   where i.SchoolId == schoolId
                       && i.Date >= start
                       && i.Date <= end
                   select new
                   {
                       r.Code,
                       r.Class,
                       r.Description,
                       Date = i.Date,
                       InspectionId = i.Id,
                       FormId = i.FormId,
                       ItemNo = item.No,
                       ind.Status,
                       ind.Remarks,
                   }).ToListAsync();

            var reportMap = new Dictionary<string, FormReportRowDto>();

            foreach (var row in rows)
            {
                var date = row.Date.ToString("yyyy-MM-dd");
                if (!reportMap.ContainsKey(row.Code))
                {
                    reportMap[row.Code] = new FormReportRowDto
                    {
                        Code = row.Code,
                        Class = row.Class,
                        Description = row.Description,
                    };
                }

                var item = new FormReportItemDto
                {
                    Date = date,
                    InspectionId = row.InspectionId,
                    FormId = row.FormId,
                    ItemNo = row.ItemNo,
                    Remarks = row.Remarks,
                };

                var isIn = reportMap[row.Code].Pass.Any(r => r.Date == date && r.FormId == row.FormId && r.ItemNo == row.ItemNo)
                    || reportMap[row.Code].Fail.Any(r => r.Date == date && r.FormId == row.FormId && r.ItemNo == row.ItemNo)
                    || reportMap[row.Code].Others.Any(r => r.Date == date && r.FormId == row.FormId && r.ItemNo == row.ItemNo);

                if (isIn) continue;

                if (row.Status == "pass") reportMap[row.Code].Pass.Add(item);
                else if (row.Status == "fail") reportMap[row.Code].Fail.Add(item);
                else reportMap[row.Code].Others.Add(item);
            }
            return Ok(reportMap.Values.ToList());
        }

        //GET api/reports/visiting
        [HttpGet("visiting")]

        public async Task<IActionResult> getVisitingReport(DateTime start, DateTime end)
        {
            var schoolId = GetSchoolId();

            var rows = await (
                from i in _db.Inspections
                join ind in _db.InspectionDetails on i.Id equals ind.InspectionId
                join item in _db.Items on ind.ItemId equals item.Id
                join iv in _db.ItemVisitingForms on item.Id equals iv.ItemId
                join v in _db.VisitingForms on iv.VisitingFormId equals v.Id
                where i.SchoolId == schoolId
                       && i.Date >= start
                       && i.Date <= end
                select new
                {
                    v.Code,
                    v.Class,
                    v.Description,
                    Date = i.Date,
                    InspectionId = i.Id,
                    FormId = i.FormId,
                    ItemNo = item.No,
                    ind.Status,
                    ind.Remarks,
                }).ToListAsync();

            var reportMap = new Dictionary<string, FormReportRowDto>();

            foreach (var row in rows)
            {
                var date = row.Date.ToString("yyyy-MM-dd");
                if (!reportMap.ContainsKey(row.Code))
                {
                    reportMap[row.Code] = new FormReportRowDto
                    {
                        Code = row.Code,
                        Class = row.Class,
                        Description = row.Description,
                    };
                }

                var item = new FormReportItemDto
                {
                    Date = date,
                    InspectionId = row.InspectionId,
                    FormId = row.FormId,
                    ItemNo = row.ItemNo,
                    Remarks = row.Remarks,
                };

                var isIn = reportMap[row.Code].Pass.Any(r => r.Date == date && r.FormId == row.FormId && r.ItemNo == row.ItemNo)
                    || reportMap[row.Code].Fail.Any(r => r.Date == date && r.FormId == row.FormId && r.ItemNo == row.ItemNo)
                    || reportMap[row.Code].Others.Any(r => r.Date == date && r.FormId == row.FormId && r.ItemNo == row.ItemNo);

                if (isIn) continue;

                if (row.Status == "pass") reportMap[row.Code].Pass.Add(item);
                else if (row.Status == "fail") reportMap[row.Code].Fail.Add(item);
                else reportMap[row.Code].Others.Add(item);
            }
            return Ok(reportMap.Values.ToList());
        }

        // GET api/reports/prodtemp
        [HttpGet("prodtemp")]

        public async Task<IActionResult> GetTempOfProdReport(DateTime start, DateTime end)
        {
            var schoolId = GetSchoolId();
            var targetNos = new[] { "8-01", "8-03", "8-04", "8-05", "8-06" };
            var prodLabelMap = new Dictionary<string, string>
            {
                { "8-01", "成品確實封蓋" },
                { "8-03", "主食" },
                { "8-04", "主菜" },
                { "8-05", "副菜" },
                { "8-06", "青菜" },
            };

            var prodAttrMap = new Dictionary<string, string>
            {
                { "8-01", "wasCovered" },
                { "8-03", "starter" },
                { "8-04", "mainCourse" },
                { "8-05", "sideDish" },
                { "8-06", "vegetable" },
            };

            var rows = await (
                 from i in _db.Inspections
                 join ind in _db.InspectionDetails on i.Id equals ind.InspectionId
                 join item in _db.Items on ind.ItemId equals item.Id
                 where i.SchoolId == schoolId
                    && i.Date >= start
                    && i.Date <= end
                    && targetNos.Contains(item.No)
                 select new
                 {
                     Date = i.Date,
                     item.No,
                     ind.Status,
                     ind.CheckValue,
                     ind.Remarks,
                 }
                ).ToListAsync();

            var rowMap = new Dictionary<string, Dictionary<string, object?>>();
            var abnormalRows = new List<AbnormalRowDto>();

            foreach (var row in rows)
            {
                var date = row.Date.ToString("yyyy-MM-dd");

                if (!rowMap.ContainsKey(date))
                {
                    rowMap[date] = new Dictionary<string, object?>
            {
                { "date", row.Date }
            };
                }

                if (rowMap[date].ContainsKey(row.No)) continue;

                if (row.Status != "pass")
                {
                    abnormalRows.Add(new AbnormalRowDto
                    {
                        Date = date,
                        No = row.No,
                        Category = "",
                        Item = prodLabelMap[row.No],
                        Status = row.Status,
                        Remarks = row.Remarks,
                    });
                }

                if (row.No == "8-01")
                {
                    rowMap[date][prodAttrMap[row.No]] = row.Status == "pass";
                }
                else
                {
                    rowMap[date][prodAttrMap[row.No]] = row.CheckValue;
                }
            }

            return Ok(new CheckValueReportDto
            {
                Rows = rowMap.Values.ToList(),
                AbnormalRows = abnormalRows,
            });
        }

        // GET api/reports/tnh
        [HttpGet("tnh")]
        public async Task<IActionResult> GetTnhReport(DateTime start, DateTime end)
        {
            var schoolId = GetSchoolId();

            var targetNos = new[] { "9-03", "9-04", "6-02", "6-01", "8-08" };

            var tnhLabelMap = new Dictionary<string, string>
            {
                { "9-03", "庫房溫度" },
                { "9-04", "庫房濕度" },
                { "6-02", "食材冰箱冷藏溫度" },
                { "6-01", "食材冰箱冷凍溫度" },
                { "8-08", "檢體冰箱冷藏溫度" },
            };

            var tnhAttrMap = new Dictionary<string, string>
            {
                { "9-03", "warehouseTemp" },
                { "9-04", "warehouseHum" },
                { "6-02", "fridgeCold" },
                { "6-01", "fridgeFreeze" },
                { "8-08", "specimenFridge" },
            };

            var rows = await (
                 from i in _db.Inspections
                 join ind in _db.InspectionDetails on i.Id equals ind.InspectionId
                 join item in _db.Items on ind.ItemId equals item.Id
                 where i.SchoolId == schoolId
                     && i.Date >= start
                     && i.Date <= end
                     && targetNos.Contains(item.No)
                 select new
                 {
                     Date = i.Date,
                     item.No,
                     ind.Status,
                     ind.CheckValue,
                     ind.Remarks,
                 }
             ).ToListAsync();

            var rowMap = new Dictionary<string, Dictionary<string, object?>>();
            var abnormalRows = new List<AbnormalRowDto>();

            foreach (var row in rows)
            {
                var date = row.Date.ToString("yyyy-MM-dd");

                if (!rowMap.ContainsKey(date))
                {
                    rowMap[date] = new Dictionary<string, object?>
        {
            { "date", row.Date }
        };
                }

                if (rowMap[date].ContainsKey(row.No)) continue;

                if (row.Status != "pass")
                {
                    abnormalRows.Add(new AbnormalRowDto
                    {
                        Date = date,
                        No = row.No,
                        Category = "",
                        Item = tnhLabelMap[row.No],
                        Status = row.Status,
                        Remarks = row.Remarks,
                    });
                }

                rowMap[date][tnhAttrMap[row.No]] = row.CheckValue ?? "";
            }

            return Ok(new CheckValueReportDto
            {
                Rows = rowMap.Values.ToList(),
                AbnormalRows = abnormalRows,
            });
        }

        // GET api/reports/tableware
        [HttpGet("tableware")]
        public async Task<IActionResult> GetTablewareReport(DateTime start, DateTime end)
        {
            var schoolId = GetSchoolId();

            var targetNos = new[] { "6-08", "6-09", "6-10", "6-11", "6-12", "6-13", "6-14", "6-15", "6-16" };

            var tablewareLabelMap = new Dictionary<string, string>
            {
                { "6-08", "餐桶外觀" },
                { "6-09", "餐桶澱粉殘留檢驗" },
                { "6-10", "餐桶脂肪殘留檢驗" },
                { "6-11", "湯桶外觀" },
                { "6-12", "湯桶澱粉殘留檢驗" },
                { "6-13", "湯桶脂肪殘留檢驗" },
                { "6-14", "餐具外觀" },
                { "6-15", "餐具澱粉殘留檢驗" },
                { "6-16", "餐具脂肪殘留檢驗" },
            };

            var tablewareAttrMap = new Dictionary<string, string>
            {
                { "6-08", "diningBucketLook" },
                { "6-09", "diningBucketStarch" },
                { "6-10", "diningBucketFat" },
                { "6-11", "soupBucketLook" },
                { "6-12", "soupBucketStarch" },
                { "6-13", "soupBucketFat" },
                { "6-14", "tablewareLook" },
                { "6-15", "tablewareStarch" },
                { "6-16", "tablewareFat" },
            };

            var rows = await (
                from i in _db.Inspections
                join ind in _db.InspectionDetails on i.Id equals ind.InspectionId
                join item in _db.Items on ind.ItemId equals item.Id
                where i.SchoolId == schoolId
                    && i.Date >= start
                    && i.Date <= end
                    && targetNos.Contains(item.No)
                select new
                {
                    Date = i.Date,
                    item.No,
                    ind.Status,
                    ind.Remarks,
                }
            ).ToListAsync();

            var rowMap = new Dictionary<string, Dictionary<string, object?>>();
            var abnormalRows = new List<AbnormalRowDto>();

            foreach (var row in rows)
            {
                var date = row.Date.ToString("yyyy-MM-dd");

                if (!rowMap.ContainsKey(date))
                {
                    rowMap[date] = new Dictionary<string, object?>
            {
                { "date", row.Date }
            };
                }

                if (rowMap[date].ContainsKey(row.No)) continue;

                if (row.Status != "pass")
                {
                    abnormalRows.Add(new AbnormalRowDto
                    {
                        Date = date,
                        No = row.No,
                        Category = "",
                        Item = tablewareLabelMap[row.No],
                        Status = row.Status,
                        Remarks = row.Remarks,
                    });
                }

                rowMap[date][tablewareAttrMap[row.No]] = row.Status == "pass";
            }

            return Ok(new CheckValueReportDto
            {
                Rows = rowMap.Values.ToList(),
                AbnormalRows = abnormalRows,
            });
        }
    }
}