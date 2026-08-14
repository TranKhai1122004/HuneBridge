using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HuneBridge.Controllers
{
    [ApiController]
    [Route("api/hune")]
    public class HuneController : ControllerBase
    {
        private readonly ILogger<HuneController> _logger;
        private readonly IConfiguration _configuration;

        public HuneController(ILogger<HuneController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("read-card")]
        public IActionResult ReadCard(
            [FromQuery] int? com,
            [FromQuery] int? nBlock,
            [FromQuery] int? encrypt,
            [FromQuery] string? cardPass,
            [FromQuery] string? systemCode)
        {
            try
            {
                var usedCom = com ?? _configuration.GetValue("Hune:Com", 5);
                var usedBlock = nBlock ?? _configuration.GetValue("Hune:nBlock", 4);
                var usedEncrypt = encrypt ?? _configuration.GetValue("Hune:Encrypt", 1);
                var usedPass = cardPass ?? _configuration["Hune:CardPass"] ?? "82A094FFFFFF";
                var usedCode = systemCode ?? _configuration["Hune:SystemCode"] ?? "68512554";

                var cardSN = new StringBuilder(64);
                int retSerial = HuneNative.ReadCardSN(usedCom, cardSN);

                int cardNoOld = 0;
                int cardType = 0;
                int level = 1;
                var pass = new StringBuilder(64);
                var code = new StringBuilder(64);
                var address = new StringBuilder(64);
                var datetime = new StringBuilder(64);
                pass.Append(usedPass);
                code.Append(usedCode);

                int retCardNo = HuneNative.ReadMessage(
                    usedCom, usedBlock, usedEncrypt,
                    ref cardNoOld, ref cardType, ref level,
                    pass, code, address, datetime);

                _logger.LogInformation(
                    "ReadCardSN={retSerial}, ReadMessage={retCardNo}, cardSN={cardSN}, cardNoOld={cardNoOld}",
                    retSerial, retCardNo, cardSN.ToString(), cardNoOld);

                return Ok(new
                {
                    retSerial,
                    retCardNo,
                    cardSN = cardSN.ToString(),
                    cardNoOld
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi DLL ReadCard");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("key-card")]
        public IActionResult WriteKeyCard([FromBody] KeyCardRequest req)
        {
            try
            {
                _logger.LogInformation("KeyCard CardNo={CardNo}, RoomPass={RoomPass}, Com={Com}", req.CardNo, req.RoomPass, req.Com);

                int ret = HuneNative.KeyCard(
                    req.Com, req.CardNo, req.NBlock, req.Encrypt, req.CardPass, req.SystemCode, req.HotelCode,
                    req.RoomPass, req.Address, req.SdIn, req.StIn, req.SdOut, req.StOut,
                    req.LevelPass, req.PassMode, req.AddressMode, req.AddressQty, req.TimeMode,
                    req.V8, req.V16, req.V24, req.AlwaysOpen, req.OpenBolt, req.TerminateOld, req.ValidTimes);

                _logger.LogInformation("KeyCard ret={ret}", ret);
                return Ok(new { ret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi DLL KeyCard");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hune Bridge Service is running on Port 5050!");
        }
    }

    public record KeyCardRequest(
        int CardNo, int NBlock, int Encrypt, string CardPass, string SystemCode, string HotelCode,
        string RoomPass, string Address, string SdIn, string StIn, string SdOut, string StOut,
        int LevelPass, int PassMode, int AddressMode, int AddressQty, int TimeMode,
        int V8, int V16, int V24, int AlwaysOpen, int OpenBolt, int TerminateOld, int ValidTimes, int Com);
}
