#! /bin/bash

DATABASE=<database>
USERNAME=<username>
PASSWORD=<password>

cd `dirname $0`

PARTITION_NAME=`date -d '1 day' '+%Y%m%d'`
TARGET_DATE=`date -d '2 day' '+%Y-%m-%d'`

echo "ALTER TABLE Binance.PriceStatsSecond ADD PARTITION (PARTITION p${PARTITION_NAME} VALUES LESS THAN ('${TARGET_DATE}') COMMENT = '${TARGET_DATE}');" > alter_table.sql

mysql -u ${USERNAME} -p${PASSWORD} -D ${DATABASE} < alter_table.sql >> partition.log
