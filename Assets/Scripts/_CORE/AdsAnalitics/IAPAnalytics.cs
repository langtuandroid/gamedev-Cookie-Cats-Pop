using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Prime31;
using TactileModules.Facebook.Plugins;

public class IAPAnalytics
{
    public static void LogUnvalidatedPurchase(IAPAnalytics.TransactionEventData transactionEventData, string transactionSignature, string iapProvider)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.UnvalidatedTransactionEvent(transactionEventData, transactionSignature, iapProvider), -1.0, null);
    }

    public static void LogValidatedPurchase(IAPAnalytics.TransactionEventData transactionEventData, bool isSandbox)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.TransactionEvent(transactionEventData, isSandbox), -1.0, null);
        double price;
        string currencyCode;
        if (transactionEventData.p.StoreInfo != null && double.TryParse(transactionEventData.p.StoreInfo.price, out price))
        {
            currencyCode = transactionEventData.p.StoreInfo.currencyCode;
        }
        else
        {
            currencyCode = transactionEventData.p.TactileInfo.CurrencyCode;
            price = transactionEventData.p.TactileInfo.Price;
        }

        AdjustUtils.LogValidatedPurchase(transactionEventData.p);
    }

    public static void LogPurchaseStarted(string purchaseSessionId, InAppProduct p)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseStartedEvent(purchaseSessionId, p), -1.0, null);
    }

    public static void LogPurchaseFailed(string purchaseSessionId, InAppProduct p, string errorMessage)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseFailedEvent(purchaseSessionId, p, errorMessage), -1.0, null);
    }

    public static void LogPurchaseInitiated(string purchaseSessionId, InAppProduct p)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseInitiatedEvent(purchaseSessionId, p), -1.0, null);
    }

    public static void LogPurchaseCancelled(string purchaseSessionId, InAppProduct p, string errorMessage)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseCancelledEvent(purchaseSessionId, p, errorMessage), -1.0, null);
    }

    public static void LogPurchaseSucceeded(string purchaseSessionId, InAppProduct p, string transactionId)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseSucceededEvent(purchaseSessionId, p, transactionId), -1.0, null);
    }

    public static void LogPurchaseSuccessfulUnknownIdentifier(string productId, string purchaseSessionId, string transactionId, string transactionData)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseSuccessfulUnknownIdentifier(productId, purchaseSessionId, transactionId, transactionData), -1.0, null);
    }

    public static void LogPurchaseConsumeFailed(GooglePurchase purchase)
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.PurchaseConsumeFailedEvent(purchase), -1.0, null);
    }

    public static void LogUserCheated()
    {
        TactileAnalytics.Instance.LogEvent(new IAPAnalytics.UserCheatedEvent(), -1.0, null);
        AdjustUtils.LogUserCheated();
    }

    [TactileAnalytics.EventAttribute("transaction", true)]
    private class TransactionEvent : IAPAnalytics.TransactionBasicEvent
    {
        public TransactionEvent(IAPAnalytics.TransactionEventData transactionEventData, bool isSandbox) : base(transactionEventData)
        {
            this.IsSandbox = isSandbox;
        }

        private TactileAnalytics.OptionalParam<bool> IsSandbox { [UsedImplicitly] get; set; }
    }

    [TactileAnalytics.EventAttribute("unvalidatedTransaction", true)]
    private class UnvalidatedTransactionEvent : IAPAnalytics.TransactionBasicEvent
    {
        public UnvalidatedTransactionEvent(IAPAnalytics.TransactionEventData transactionEventData, string transactionSignature, string iapProvider) : base(transactionEventData)
        {
            this.TransactionSignature = transactionSignature;
            this.ValidateTransaction = true;
            this.IAPProvider = iapProvider;
            this.BundleIdentifier = SystemInfoHelper.BundleIdentifier;
            this.AdvertisingId = SystemInfoHelper.AdvertisingId;
            this.FacebookAttributionId = FacebookAndroid.GetAttributionId();
        }

        private TactileAnalytics.OptionalParam<bool> ValidateTransaction { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> TransactionSignature { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> IAPProvider { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> BundleIdentifier { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> AdvertisingId { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> FacebookAttributionId { [UsedImplicitly] get; set; }
    }

    private class TransactionBasicEvent : BasicEvent
    {
        public TransactionBasicEvent(IAPAnalytics.TransactionEventData transactionEventData)
        {
            this.TransactionName = "IAP - " + transactionEventData.p.Title;
            this.ProductId = transactionEventData.p.Identifier;
            this.PurchaseSessionId = transactionEventData.purchaseSessionId;
            this.TransactionId = transactionEventData.transactionId;
            this.TransactionReceipt = transactionEventData.transactionReceipt;
            this.ClientPurchasedTimestamp = transactionEventData.clientPurchasedTimestamp;
            double value;
            if (transactionEventData.p.StoreInfo != null && double.TryParse(transactionEventData.p.StoreInfo.price, out value))
            {
                this.LocalCurrencyCode = transactionEventData.p.StoreInfo.currencyCode;
                this.LocalPrice = value;
            }
            this.Price = (int)(transactionEventData.p.TactileInfo.Price * 100.0);
            if (!string.IsNullOrEmpty(transactionEventData.orderId))
            {
                this.OrderId = transactionEventData.orderId;
            }
        }

        private TactileAnalytics.RequiredParam<string> TransactionName { [UsedImplicitly] get; set; }

        private TactileAnalytics.RequiredParam<string> ProductId { [UsedImplicitly] get; set; }

        private TactileAnalytics.RequiredParam<string> PurchaseSessionId { [UsedImplicitly] get; set; }

        private TactileAnalytics.RequiredParam<string> TransactionId { [UsedImplicitly] get; set; }

        private TactileAnalytics.RequiredParam<string> TransactionReceipt { [UsedImplicitly] get; set; }

        private TactileAnalytics.RequiredParam<int> Price { [UsedImplicitly] get; set; }

        private TactileAnalytics.RequiredParam<DateTime> ClientPurchasedTimestamp { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> LocalCurrencyCode { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<double> LocalPrice { [UsedImplicitly] get; set; }

        private TactileAnalytics.OptionalParam<string> OrderId { [UsedImplicitly] get; set; }
    }

    public class TransactionEventData
    {
        public TransactionEventData(InAppProduct p, int amount, string purchaseSessionId, string transactionId, string transactionReceipt, string orderId, DateTime clientPurchasedTimestamp)
        {
            this.p = p;
            this.amount = amount;
            this.purchaseSessionId = purchaseSessionId;
            this.transactionId = transactionId;
            this.transactionReceipt = transactionReceipt;
            this.orderId = orderId;
            this.clientPurchasedTimestamp = clientPurchasedTimestamp;
        }

        public readonly InAppProduct p;

        public readonly int amount;

        public readonly string purchaseSessionId;

        public readonly string transactionId;

        public readonly string transactionReceipt;

        public readonly string orderId;

        public readonly DateTime clientPurchasedTimestamp;
    }

    [TactileAnalytics.EventAttribute("purchaseStarted", true)]
    private class PurchaseStartedEvent : IAPAnalytics.PurchaseBasicEvent
    {
        public PurchaseStartedEvent(string purchaseSessionId, InAppProduct p) : base(purchaseSessionId, p)
        {
        }
    }

    [TactileAnalytics.EventAttribute("purchaseFailed", true)]
    private class PurchaseFailedEvent : IAPAnalytics.PurchaseBasicEvent
    {
        public PurchaseFailedEvent(string purchaseSessionId, InAppProduct p, string error) : base(purchaseSessionId, p)
        {
            this.Error = error;
        }

        private TactileAnalytics.RequiredParam<string> Error { get; set; }
    }

    [TactileAnalytics.EventAttribute("purchaseInitiated", false)]
    private class PurchaseInitiatedEvent : IAPAnalytics.PurchaseBasicEvent
    {
        public PurchaseInitiatedEvent(string purchaseSessionId, InAppProduct p) : base(purchaseSessionId, p)
        {
        }
    }

    [TactileAnalytics.EventAttribute("purchaseCancelled", true)]
    private class PurchaseCancelledEvent : IAPAnalytics.PurchaseBasicEvent
    {
        public PurchaseCancelledEvent(string purchaseSessionId, InAppProduct p, string error) : base(purchaseSessionId, p)
        {
            this.Error = error;
        }

        private TactileAnalytics.RequiredParam<string> Error { get; set; }
    }

    [TactileAnalytics.EventAttribute("purchaseSucceeded", true)]
    private class PurchaseSucceededEvent : IAPAnalytics.PurchaseBasicEvent
    {
        public PurchaseSucceededEvent(string purchaseSessionId, InAppProduct p, string transactionId) : base(purchaseSessionId, p)
        {
            this.TransactionId = transactionId;
        }

        private TactileAnalytics.RequiredParam<string> TransactionId { get; set; }
    }

    private class PurchaseBasicEvent : BasicEvent
    {
        public PurchaseBasicEvent(string purchaseSessionId, InAppProduct p)
        {
            this.PurchaseSessionId = purchaseSessionId;
            this.ProductId = p.Identifier;
            double value;
            if (p.StoreInfo != null && double.TryParse(p.StoreInfo.price, out value))
            {
                this.LocalCurrencyCode = p.StoreInfo.currencyCode;
                this.LocalPrice = value;
            }
            this.Price = (int)(p.TactileInfo.Price * 100.0);
        }

        private TactileAnalytics.RequiredParam<string> PurchaseSessionId { get; set; }

        private TactileAnalytics.RequiredParam<string> ProductId { get; set; }

        private TactileAnalytics.RequiredParam<int> Price { get; set; }

        private TactileAnalytics.OptionalParam<string> LocalCurrencyCode { get; set; }

        private TactileAnalytics.OptionalParam<double> LocalPrice { get; set; }
    }

    [TactileAnalytics.EventAttribute("purchaseConsumeFailed", false)]
    private class PurchaseConsumeFailedEvent : BasicEvent
    {
        public PurchaseConsumeFailedEvent(GooglePurchase purchase)
        {
            this.ProductId = purchase.productId;
            this.TransactionId = purchase.purchaseToken;
            this.TransactionReceipt = purchase.originalJson;
            this.OrderId = purchase.orderId;
            this.PurchaseState = purchase.purchaseState.ToString();
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.ProviderPurchaseTime = dateTime.AddMilliseconds((double)purchase.purchaseTime);
        }

        private TactileAnalytics.RequiredParam<string> ProductId { get; set; }

        private TactileAnalytics.RequiredParam<string> TransactionId { get; set; }

        private TactileAnalytics.RequiredParam<string> TransactionReceipt { get; set; }

        private TactileAnalytics.RequiredParam<string> OrderId { get; set; }

        private TactileAnalytics.RequiredParam<DateTime> ProviderPurchaseTime { get; set; }

        private TactileAnalytics.RequiredParam<string> PurchaseState { get; set; }
    }

    [TactileAnalytics.EventAttribute("purchaseSuccessfulUnknownIdentifier", false)]
    private class PurchaseSuccessfulUnknownIdentifier : BasicEvent
    {
        public PurchaseSuccessfulUnknownIdentifier(string productId, string purchaseSessionId, string transactionId, string transactionData)
        {
            this.ProductId = productId;
            this.PurchaseSessionId = purchaseSessionId;
            this.TransactionId = transactionId;
            this.TransactionData = transactionData;
        }

        private TactileAnalytics.RequiredParam<string> ProductId { get; set; }

        private TactileAnalytics.RequiredParam<string> PurchaseSessionId { get; set; }

        private TactileAnalytics.RequiredParam<string> TransactionId { get; set; }

        private TactileAnalytics.RequiredParam<string> TransactionData { get; set; }
    }

    [TactileAnalytics.EventAttribute("userCheated", true)]
    protected class UserCheatedEvent : BasicEvent
    {
    }
}
