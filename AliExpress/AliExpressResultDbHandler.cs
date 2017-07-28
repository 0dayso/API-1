using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Handler;
using API.Helper;


namespace AliExpress
{
    internal class AliExpressResultDbHandler:ResultDbHandler
    {

        public AliExpressResultDbHandler(MySqlHelper resultHelper) : base(resultHelper)
        {
        }

        public AliExpressResultDbHandler(string resultDbServer, string resultDbName, string resultDbUserName, string resultDbPassWord) : base(resultDbServer, resultDbName, resultDbUserName, resultDbPassWord)
        {
        }


        /// <summary>
        /// CreateTable
        /// </summary>
        /// <param name="tableName"></param>
        public override void CreateTable(string tableName)
        {
            var sql = $@"CREATE TABLE {tableName}
                        (
	                        amount DOUBLE COMMENT '金额',
	                        cent DOUBLE COMMENT '金额分',
	                        currencyCode VARCHAR(128) COMMENT '币种USD/RUB',
	                        sellerSignerFullname VARCHAR(128) COMMENT '卖家全名',
	                        buyerLoginId VARCHAR(128) COMMENT '买家登录信息',
	                        paymentType VARCHAR(128) COMMENT '支付类型',
	                        orderStatus VARCHAR(128) COMMENT '订单状态',
	                        orderId VARCHAR(128) COMMENT '订单编号',
	                        issueStatus VARCHAR(128) COMMENT '纠纷状况',
	                        sendGoodsTime VARCHAR(128) COMMENT '发货时间',
	                        gmtPayTime VARCHAR(128) COMMENT '支付时间',
	                        gmtCreate VARCHAR(128) COMMENT '订单创建时间',
	                        fundStatus VARCHAR(128) COMMENT '资金状态',
	                        frozenStatus VARCHAR(128) COMMENT '冻结状态',
	                        loanAmount VARCHAR(128) COMMENT '放款金额',
	                        escrowFee VARCHAR(128) COMMENT '手续费',
	                        buyerSignerFullname VARCHAR(128) COMMENT '买家全名',
	                        bizType VARCHAR(128) COMMENT '订单类型',
	                        productList TEXT COMMENT '产品列表',
                            PRIMARY KEY(orderId)
                        )";

            ResultHelper.CreateTable(sql);
        }

        
    }
}
