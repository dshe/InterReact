﻿using Microsoft.Extensions.Logging;
using Stringification;
using System.IO;

namespace InterReact;

internal sealed class ResponseMessageComposer
{
    private ILogger Logger { get; }
    private ResponseReader Reader { get; }

    internal ResponseMessageComposer(InterReactClientConnector connector)
    {
        Logger = connector.Logger;
        Reader = new ResponseReader(connector);
    }

    internal object ComposeMessage(string[] strings)
    {
        string code = "";
        try
        {
            Reader.SetEnumerator(strings);
            code = Reader.ReadString();
            object message = GetResponseMessage(code, Reader);
            Reader.VerifyEnumerationEnd();
            return message;
        }
        catch (Exception e)
        {
            string m = $"ResponseComposer error: [{code} => {strings.JoinStrings(", ")}].";
            Logger.LogError(e, "{Message}", m);
            throw new InvalidDataException(m, e);
        }
    }

    private static object GetResponseMessage(string code, ResponseReader reader) => code switch
    {
        "1" => new PriceTick(reader),
        "2" => new SizeTick(reader),
        "3" => new OrderStatusReport(reader),
        "4" => new Alert(reader),
        "5" => new OpenOrder(reader),
        "6" => new AccountValue(reader),
        "7" => new PortfolioValue(reader),
        "8" => new AccountUpdateTime(reader),
        "9" => new NextOrderId(reader),
        "10" => new ContractDetails(reader, ContractDetailsType.GeneralContractType),
        "11" => new Execution(reader),
        "12" => new MarketDepth(reader, false),
        "13" => new MarketDepth(reader, true),
        "14" => new NewsBulletin(reader),
        "15" => new ManagedAccounts(reader),
        "16" => new FinancialAdvisor(reader),
        "17" => new HistoricalData(reader),
        "18" => new ContractDetails(reader, ContractDetailsType.BondContractType),
        "19" => new ScannerParameters(reader),
        "20" => new ScannerData(reader),
        "21" => new OptionComputationTick(reader),

        "45" => GenericTick.Create(reader),
        "46" => StringTick.Create(reader),
        "47" => new ExchangeForPhysicalTick(reader),

        "49" => new CurrentTime(reader),
        "50" => new RealtimeBar(reader),
        "51" => new FundamentalData(reader),
        "52" => new ContractDetailsEnd(reader),
        "53" => new OpenOrderEnd(reader),
        "54" => new AccountUpdateEnd(reader),
        "55" => new ExecutionEnd(reader),
        "56" => new DeltaNeutralContract(reader, true),
        "57" => new SnapshotEndTick(reader),
        "58" => new MarketDataTickType(reader),
        "59" => new CommissionReport(reader),

        "61" => new Position(reader),
        "62" => new PositionEnd(reader),
        "63" => new AccountSummary(reader),
        "64" => new AccountSummaryEnd(reader),
        "65" => new VerifyMessageApi(reader),
        "66" => new VerifyCompleted(reader),
        "67" => new DisplayGroups(reader),
        "68" => new DisplayGroupUpdate(reader),
        "69" => new VerifyAndAuthorizeMessageApi(reader),
        "70" => new VerifyAndAuthorizeCompleted(reader),
        "71" => new AccountPositionMulti(reader),
        "72" => new AccountPositionMultiEnd(reader),
        "73" => new AccountUpdateMulti(reader),
        "74" => new AccountUpdateMultiEnd(reader),
        "75" => new SecurityDefinitionOptionParameter(reader),
        "76" => new SecurityDefinitionOptionParameterEnd(reader),
        "77" => new SoftDollarTiers(reader),
        "78" => new FamilyCodes(reader),
        "79" => new SymbolSamples(reader),
        "80" => new MarketDepthExchanges(reader),
        "81" => new TickRequestParams(reader),
        "82" => new SmartComponents(reader),
        "83" => new NewsArticle(reader),
        "84" => new TickNews(reader),
        "85" => new NewsProviders(reader),
        "86" => new HistoricalNews(reader),
        "87" => new HistoricalNewsEnd(reader),
        "88" => new HeadTimestamp(reader),
        "89" => new HistogramData(reader),
        "90" => new HistoricalDataUpdate(reader),
        "91" => new RerouteMktData(reader),
        "92" => new RerouteMktDepth(reader),
        "93" => new MarketRule(reader),
        "94" => new PnL(reader),
        "95" => new PnLSingle(reader),
        "96" => new HistoricalTicks(reader),
        "97" => new HistoricalBidAskTicks(reader),
        "98" => new HistoricalLastTicks(reader),
        "99" => TickByTick.Create(reader),
        "100" => new OrderBound(reader),
        "101" => new CompletedOrder(reader),
        "102" => new CompletedOrdersEnd(),
        "103" => new ReplaceFAEnd(reader),
        "104" => new WshMetaData(reader),
        "105" => new WshEventData(reader),
        _ => throw new InvalidDataException($"Undefined code '{code}'.")
    };
}
