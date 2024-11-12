﻿using System.Diagnostics.CodeAnalysis;

namespace InterReact;

[SuppressMessage("Design", "CA1707")]
public enum ServerVersion
{
    NONE = 0,
    //FRACTIONAL_POSITIONS = 101,
    //PEGGED_TO_BENCHMARK = 102,
    //MODELS_SUPPORT = 103,
    //SEC_DEF_OPT_PARAMS_REQ = 104,
    //EXT_OPERATOR = 105,
    //SOFT_DOLLAR_TIER = 106, // max support of version 9.72
    //REQ_FAMILY_CODES = 107,
    //REQ_MATCHING_SYMBOLS = 108,
    //PAST_LIMIT = 109,
    //MD_SIZE_MULTIPLIER = 110,
    //CASH_QTY = 111,
    //REQ_MKT_DEPTH_EXCHANGES = 112,
    //TICK_NEWS = 113,
    //SMART_COMPONENTS = 114,
    //REQ_NEWS_PROVIDERS = 115,
    //REQ_NEWS_ARTICLE = 116,
    //REQ_HISTORICAL_NEWS = 117,
    //REQ_HEAD_TIMESTAMP = 118,
    //REQ_HISTOGRAM_DATA = 119,
    //SERVICE_DATA_TYPE = 120,
    //AGG_GROUP = 121,
    //UNDERLYING_INFO = 122,
    //CANCEL_HEADTIMESTAMP = 123,
    //SYNT_REALTIME_BARS = 124,
    //CFD_REROUTE = 125,
    //MARKET_RULES = 126,
    //PNL = 127,
    //NEWS_QUERY_ORIGINS = 128,
    //UNREALIZED_PNL = 129,
    //HISTORICAL_TICKS = 130,
    //MARKET_CAP_PRICE = 131,
    //PRE_OPEN_BID_ASK = 132,
    //REAL_EXPIRATION_DATE = 134,
    //REALIZED_PNL = 135,
    //LAST_LIQUIDITY = 136,
    //TICK_BY_TICK = 137,
    //DECISION_MAKER = 138,
    //MIFID_EXECUTION = 139,
    //TICK_BY_TICK_IGNORE_SIZE = 140,
    //AUTO_PRICE_FOR_HEDGE = 141,
    //WHAT_IF_EXT_FIELDS = 142,
    //SCANNER_GENERIC_OPTS = 143,
    //API_BIND_ORDER = 144,
    //ORDER_CONTAINER = 145,
    //SMART_DEPTH = 146,
    //REMOVE_NULL_ALL_CASTING = 147,
    //D_PEG_ORDERS = 148,
    //MKT_DEPTH_PRIM_EXCHANGE = 149,
    //COMPLETED_ORDERS = 150,
    //PRICE_MGMT_ALGO = 151,
    //STOCK_TYPE = 152,
    //ENCODE_MSG_ASCII7 = 153,
    //SEND_ALL_FAMILY_CODES = 154,
    //NO_DEFAULT_OPEN_CLOSE = 155,
    //PRICE_BASED_VOLATILITY = 156,
    //REPLACE_FA_END = 157,
    //DURATION = 158,
    //MARKET_DATA_IN_SHARES = 159,
    //POST_TO_ATS = 160,
    //WSHE_CALENDAR = 161,

    //AUTO_CANCEL_PARENT = 162,
    //FRACTIONAL_SIZE_SUPPORT = 163,
    //SIZE_RULES = 164,
    //HISTORICAL_SCHEDULE = 165,
    //ADVANCED_ORDER_REJECT = 166,
    //USER_INFO = 167,
    //CRYPTO_AGGREGATED_TRADES = 168,
    //MANUAL_ORDER_TIME = 169,
    //PEGBEST_PEGMID_OFFSETS = 170,
    //WSH_EVENT_DATA_FILTERS = 171,
    //IPO_PRICES = 172,
    //WSH_EVENT_DATA_FILTERS_DATE = 173,
    //INSTRUMENT_TIMEZONE = 174,
    //HMDS_MARKET_DATA_IN_SHARES = 175,
    BOND_ISSUERID = 176,
    FA_PROFILE_DESUPPORT = 177,
    PENDING_PRICE_REVISION = 178,
    FUND_DATA_FIELDS = 179,
    MANUAL_ORDER_TIME_EXERCISE_OPTIONS = 180,
    OPEN_ORDER_AD_STRATEGY = 181,
    LAST_TRADE_DATE = 182,
    CUSTOMER_ACCOUNT = 183,
    PROFESSIONAL_CUSTOMER = 184,
    BOND_ACCRUED_INTEREST = 185,
    INELIGIBILITY_REASONS = 186,
    RFQ_FIELDS = 187
}
