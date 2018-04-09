
namespace API.CP.BASE
{

    public sealed class caika_object_type : AosuApp.object_type
    {
        public const int CAIKA_BANKCARD = 201;

        /// <summary>
        /// 彩咖网对象类型 - 起始编号
        /// </summary>
        public const int CAIKA_START = 200;
        /// <summary>
        /// 彩咖网对象类型 - 结束编号
        /// </summary>
        public const int CAIKA_END = 299;
    }

    public sealed class activity_type
    {
        /// <summary>
        /// 单次活动
        /// </summary>
        public const string ONCE = "A";
        /// <summary>
        /// 周期性活动
        /// </summary>
        public const string PERIOD = "B";
    }

    public sealed class activity_scope
    {
        public const string for_agent = "A";

        public const string for_member = "B";

        public const string both = "C";
    }

    public sealed class deposit_type
    {
        public const string netbank = "netbank";

        public const string fastway = "fastway";

        public const string thirdway = "thirdway";
    }

    public static class detail_type
    {
        public const string summary = "0";
        public const string basic = "1";
        public const string clash = "2";
        public const string history1 = "3";
        public const string history2 = "4";
        public const string board1_all = "5";
        public const string board2_all = "6";
        public const string board1_home = "7";
        public const string board2_home = "8";
        public const string sche1 = "9";
        public const string sche2 = "a";
        public const string chart = "b";
    }

    public static class message_id
    {
        public const int System_Start = 0;
        public const int System_End = 999;

        public const int Account_Start = 1000;
        public const int Account_KeyIn_DetailInfo = 1001;
        public const int Account_KeyIn_Withdrawals = 1002;
        public const int Account_KeyIn_BankCard = 1003;
        public const int Account_End = 1099;

        public const int UserRequest_Start = 1100;
        public const int UserRequest_ChangeMemberInfo = 1101;
        public const int UserRequest_ChangePayPassword = 1102;
        public const int UserRequest_End = 1109;
    }

    public static class account_type
    {
        public const string NormalAcount = "N";

        public const string AgentAccount = "A";

        public const string VirtualAccount = "V";

        public const string ManageAccount = "M";
    }

    public static class member_level
    {
        // 新手
        public const int L1 = 1;

        // 铜牌会员
        public const int L2 = 2;

        // 银牌会员
        public const int L3 = 3;

        // 金牌会员
        public const int L4 = 4;

        // 钻石会员
        public const int L5 = 5;
    }

    public static class agent_level
    {
        public const int L1 = 1;
        public const int L2 = 2;
        public const int L3 = 3;
    }

    public static class message_state
    {
        public const string UnRead = "UnRead";

        public const string Read = "Read";
    }

    public static class payway
    {

    }

    public static class journal_type
    {
        public const string 充值 = "cz";

        public const string 提现 = "tx";

        public const string 投注 = "tz";

        public const string 中奖 = "zj";

        public const string 撤单 = "cd";

        public const string 返点 = "fd";

        public const string 积分兑换 = "jfdh";

        public const string 活动派送 = "hdps";

        public const string 其它 = "qt";
    }

    public static class journal_state
    {
        public const string 待审核 = "UnCheck";

        public const string 已审核 = "Checked";

        public const string 已拒批 = "Rejected";
    }
}
