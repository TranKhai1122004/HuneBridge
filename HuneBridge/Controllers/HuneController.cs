using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HuneBridge.Controllers
{
    [ApiController]
    [Route("api/hune")]
    public class HuneController : ControllerBase
    {
        private readonly ILogger<HuneController> _logger;

        public HuneController(ILogger<HuneController> logger)
        {
            _logger = logger;
        }

        // POST: http://localhost:5050/api/hune/read-card
        [HttpPost("read-card")]
        public IActionResult ReadCard([FromQuery] int com = 5)
        {
            try
            {
                int retSerial = -1;
                int retCardNo = -1;
                int cardNoOld = 0;
                byte[] cardSN = new byte[16];

                int res = HuneNative.ReadCard(com, ref retSerial, ref retCardNo, cardSN, ref cardNoOld);
                string snStr = Encoding.ASCII.GetString(cardSN).TrimEnd('\0', '\r', '\n', ' ');

                _logger.LogInformation("ReadCard res={res}, retSerial={retSerial}, retCardNo={retCardNo}, cardSN={snStr}, cardNoOld={cardNoOld}",
                    res, retSerial, retCardNo, snStr, cardNoOld);

                return Ok(new
                {
                    retSerial = retSerial,
                    retCardNo = retCardNo,
                    cardSN = snStr,
                    cardNoOld = cardNoOld
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi DLL ReadCard");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: http://localhost:5050/api/hune/key-card
        [HttpPost("key-card")]
        public IActionResult WriteKeyCard([FromBody] KeyCardRequest req)
        {
            try
            {
                _logger.LogInformation("WriteKeyCard req CardNo={CardNo}, RoomPass={RoomPass}, Com={Com}", req.CardNo, req.RoomPass, req.Com);

                int ret = HuneNative.WriteKeyCard(
                    req.CardNo, req.NBlock, req.Encrypt, req.CardPass, req.SystemCode, req.HotelCode,
                    req.RoomPass, req.Address, req.SdIn, req.StIn, req.SdOut, req.StOut,
                    req.LevelPass, req.PassMode, req.AddressMode, req.AddressQty, req.TimeMode,
                    req.V8, req.V16, req.V24, req.AlwaysOpen, req.OpenBolt, req.TerminateOld, req.ValidTimes, req.Com);

                _logger.LogInformation("WriteKeyCard ret={ret}", ret);

                return Ok(new { ret = ret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi DLL WriteKeyCard");
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // Thêm đoạn này vào bên trong HuneController class:
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hune Bridge Service is running on Port 5000!");
        }

    }

    public record KeyCardRequest(
        int CardNo, int NBlock, int Encrypt, string CardPass, string SystemCode, string HotelCode,
        string RoomPass, string Address, string SdIn, string StIn, string SdOut, string StOut,
        int LevelPass, int PassMode, int AddressMode, int AddressQty, int TimeMode,
        int V8, int V16, int V24, int AlwaysOpen, int OpenBolt, int TerminateOld, int ValidTimes, int Com);
}
