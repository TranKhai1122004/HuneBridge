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
        public IActionResult ReadCard()
        {
            try
            {
                var com = _configuration.GetValue("Hune:Com", 5);
                var nBlock = _configuration.GetValue("Hune:nBlock", 4);
                var encrypt = _configuration.GetValue("Hune:Encrypt", 1);
                var cardPass = _configuration["Hune:CardPass"] ?? "82A094FFFFFF";
                var systemCode = _configuration["Hune:SystemCode"] ?? "68512554";

                var cardSN = new StringBuilder(64);
                int retSerial = HuneNative.ReadCardSN(com, cardSN);

                int cardNoOld = 0;
                int cardType = 0;
                int level = 1;
                var pass = new StringBuilder(64);
                var code = new StringBuilder(64);
                var address = new StringBuilder(64);
                var datetime = new StringBuilder(64);
                pass.Append(cardPass);
                code.Append(systemCode);

                int retCardNo = HuneNative.ReadMessage(
                    com, nBlock, encrypt,
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
                var com = _configuration.GetValue("Hune:Com", 5);
                var nBlock = _configuration.GetValue("Hune:nBlock", 4);
                var encrypt = _configuration.GetValue("Hune:Encrypt", 1);
                var cardPass = _configuration["Hune:CardPass"] ?? "82A094FFFFFF";
                var systemCode = _configuration["Hune:SystemCode"] ?? "68512554";
                var hotelCode = _configuration["Hune:HotelCode"] ?? "853192E6";
                var timeMode = _configuration.GetValue("Hune:TimeMode", 0);
                var v8 = _configuration.GetValue("Hune:V8", 255);
                var v16 = _configuration.GetValue("Hune:V16", 255);
                var v24 = _configuration.GetValue("Hune:V24", 255);
                var validTimes = _configuration.GetValue("Hune:ValidTimes", 255);

                _logger.LogInformation("KeyCard CardNo={CardNo}, RoomPass={RoomPass}, Com={Com}", req.CardNo, req.RoomPass, com);

                int ret = HuneNative.KeyCard(
                    com, req.CardNo, nBlock, encrypt, cardPass, systemCode, hotelCode,
                    req.RoomPass, req.Address, req.SdIn, req.StIn, req.SdOut, req.StOut,
                    req.LevelPass, req.PassMode, req.AddressMode, req.AddressQty, timeMode,
                    v8, v16, v24, req.AlwaysOpen, req.OpenBolt, req.TerminateOld, validTimes);

                _logger.LogInformation("KeyCard ret={ret}", ret);
                return Ok(new { ret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi DLL KeyCard");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("machine")]
        public IActionResult Machine()
        {
            return Ok(new { computerName = Environment.MachineName });
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hune Bridge Service is running on Port 5050!");
        }
    }

    public record KeyCardRequest(
        int CardNo,
        string RoomPass,
        string Address,
        string SdIn,
        string StIn,
        string SdOut,
        string StOut,
        int LevelPass,
        int PassMode,
        int AddressMode,
        int AddressQty,
        int AlwaysOpen,
        int OpenBolt,
        int TerminateOld);
}
