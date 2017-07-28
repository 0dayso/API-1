using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Handler;
using API.Helper;

namespace Amazon
{
    internal class AmazonResultDbHandler:ResultDbHandler
    {
        public AmazonResultDbHandler(MySqlHelper resultHelper) : base(resultHelper)
        {
        }

        public AmazonResultDbHandler(string resultDbServer, string resultDbName, string resultDbUserName, string resultDbPassWord) : base(resultDbServer, resultDbName, resultDbUserName, resultDbPassWord)
        {
        }

        //public override void CreateTable(string tableName)
        //{
        //    var sql = $@"CREATE TABLE {tableName}
        //                (
        //             latestShipDate VARCHAR(128),
        //             orderType VARCHAR(128),
        //             purchaseDate VARCHAR(128),
        //             buyerEmail VARCHAR(128),
        //             amazonOrderId VARCHAR(128),
        //             lastUpdateDate VARCHAR(128),
        //             isReplacementOrder VARCHAR(128),
        //             shipServiceLevel VARCHAR(128),
        //             numberOfItemsShipped DOUBLE,
        //             orderStatus VARCHAR(128),
        //             salesChannel VARCHAR(128),
        //             isBusinessOrder VARCHAR(128),
        //             numberOfItemsUnshipped DOUBLE,
        //             paymentMethodDetails VARCHAR(256),
        //             buyerName VARCHAR(128),
        //             orderTotal VARCHAR(256),
        //             isPremiumOrder VARCHAR(128),
        //             earliestShipDate VARCHAR(128),
        //             marketplaceId VARCHAR(128),
        //       fulfillmentChannel VARCHAR(128),
        //       paymentMethod VARCHAR(128),
        //       shippingAddress VARCHAR(2048),
        //       isPrime VARCHAR(128),
        //       shipmentServiceLevelCategory VARCHAR(128),
        //       sellerOrderId VARCHAR(128),
        //                PRIMARY KEY(amazonOrderId)
        //                )";

        //    ResultHelper.CreateTable(sql);
        //}



        public override void CreateTable(string tableName)
        {
            var sql = $@"CREATE TABLE `{tableName}` (
                      `autoid` BIGINT(20) NOT NULL AUTO_INCREMENT,
                      `indbtime` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                      `amazonOrderId` VARCHAR(128) NOT NULL DEFAULT '' COMMENT '亚马逊订单编号',
                      `latestShipDate` DATETIME DEFAULT NULL COMMENT '最后物流时间',
                      `orderType` VARCHAR(128) DEFAULT NULL COMMENT '订单类型',
                      `purchaseDate` DATETIME DEFAULT NULL COMMENT '购买时间（以此作为统计基准时间）',
                      `buyerEmail` VARCHAR(128) DEFAULT NULL COMMENT '买家邮件地址',
                      `lastUpdateDate` DATETIME DEFAULT NULL COMMENT '最后更新时间',
                      `isReplacementOrder` VARCHAR(128) DEFAULT NULL COMMENT '是否补单订单',
                      `shipServiceLevel` VARCHAR(128) DEFAULT NULL COMMENT '物流服务等级',
                      `numberOfItemsShipped` DOUBLE DEFAULT NULL,
                      `orderStatus` VARCHAR(128) DEFAULT NULL COMMENT '订单状态 Canceled  Pending Shipped',
                      `salesChannel` VARCHAR(128) DEFAULT NULL COMMENT '销售平台通道',
                      `isBusinessOrder` VARCHAR(128) DEFAULT NULL COMMENT '是否企业订单（相对于普通订单）',
                      `numberOfItemsUnshipped` DOUBLE DEFAULT NULL,
                      `paymentMethodDetails` VARCHAR(256) DEFAULT NULL COMMENT '付款方式详情',
                      `buyerName` VARCHAR(128) DEFAULT NULL COMMENT '买家姓名',
                      `orderTotal` VARCHAR(256) DEFAULT NULL COMMENT '订单货币种类及金额',
                      `CurrencyCode` VARCHAR(16) DEFAULT NULL COMMENT '[需清洗] 订单货币种类',
                      `Amount` DECIMAL(12,2) DEFAULT NULL COMMENT '[需清洗] 金额',
                      `isPremiumOrder` VARCHAR(128) DEFAULT NULL COMMENT '是否PREMIUM会员订单',
                      `earliestShipDate` DATETIME DEFAULT NULL COMMENT '最早物流时间',
                      `marketplaceId` VARCHAR(128) DEFAULT NULL,
                      `fulfillmentChannel` VARCHAR(128) DEFAULT NULL COMMENT '完成通道',
                      `paymentMethod` VARCHAR(128) DEFAULT NULL COMMENT '付款方式',
                      `shippingAddress` VARCHAR(2048) DEFAULT NULL COMMENT '物流目的地信息（国家 地区 邮编 地址 收件人）',
                      `Name` VARCHAR(256) DEFAULT NULL COMMENT '收件人姓名',
                      `AddressLine1` VARCHAR(1024) DEFAULT NULL COMMENT '收件人地址1',
                      `isPrime` VARCHAR(128) DEFAULT NULL COMMENT '是否 Prime 会员',
                      `shipmentServiceLevelCategory` VARCHAR(128) DEFAULT NULL COMMENT '物流服务等级 shipServiceLevel',
                      `sellerOrderId` VARCHAR(128) DEFAULT NULL COMMENT '卖家订单编号',
                      PRIMARY KEY (`autoid`),
                      UNIQUE KEY `amazonOrderId` (`amazonOrderId`),
                      KEY `orderStatus` (`orderStatus`),
                      KEY `CurrencyCode` (`CurrencyCode`),
                      KEY `purchaseDate` (`purchaseDate`),
                      KEY `paymentMethod` (`paymentMethod`)
                    ) ENGINE=MYISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC";

            ResultHelper.CreateTable(sql);
        }
    }
}
