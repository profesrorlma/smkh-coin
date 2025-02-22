using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace SmkhCoinBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiningController : ControllerBase
    {
        private static ConcurrentDictionary<string, User> users = new ConcurrentDictionary<string, User>();

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (users.ContainsKey(user.Email))
            {
                return BadRequest("User already exists.");
            }

            users[user.Email] = user;
            return Ok(new { Message = "User registered successfully!", UserId = user.Email });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!users.ContainsKey(request.Email) || users[request.Email].Password != request.Password)
            {
                return BadRequest("Invalid email or password.");
            }

            return Ok(new { Message = "Login successful!", UserId = request.Email });
        }

        [HttpPost("start-mining")]
        public IActionResult StartMining([FromBody] MiningRequest request)
        {
            if (!users.ContainsKey(request.UserId))
            {
                return BadRequest("User not found.");
            }

            var user = users[request.UserId];
            user.StartMining();
            return Ok(new { Message = "Mining started successfully!" });
        }

        [HttpPost("add-referral")]
        public IActionResult AddReferral([FromBody] ReferralRequest request)
        {
            if (!users.ContainsKey(request.UserId) || !users.ContainsKey(request.ReferralCode))
            {
                return BadRequest("Invalid user or referral code.");
            }

            var user = users[request.UserId];
            var referrer = users[request.ReferralCode];
            user.AddReferral(referrer);
            return Ok(new { Message = "Referral added successfully!" });
        }

        [HttpPost("check-withdrawal")]
        public IActionResult CheckWithdrawal([FromBody] WithdrawalRequest request)
        {
            if (!users.ContainsKey(request.UserId))
            {
                return BadRequest("User not found.");
            }

            var user = users[request.UserId];
            return Ok(new { Message = user.CheckWithdrawal() });
        }
    }

    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public double Balance { get; set; }
        public double MiningSpeed { get; set; }
        public int Referrals { get; set; }

        public void StartMining()
        {
            // Simulate mining 1 coin every 24 hours
            System.Timers.Timer timer = new System.Timers.Timer(24 * 60 * 60 * 1000);
            timer.Elapsed += (sender, e) => Balance += MiningSpeed;
            timer.Start();
        }

        public void AddReferral(User referrer)
        {
            Referrals++;
            MiningSpeed += 0.1;
            referrer.Referrals++;
        }

        public string CheckWithdrawal()
        {
            return Balance >= 500 ? "You can withdraw your SMKH Coins!" : $"You need {500 - Balance} more coins to withdraw.";
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class MiningRequest
    {
        public string UserId { get; set; }
    }

    public class ReferralRequest
    {
        public string UserId { get; set; }
        public string ReferralCode { get; set; }
    }

    public class WithdrawalRequest
    {
        public string UserId { get; set; }
    }
}