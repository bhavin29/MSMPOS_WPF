--SELECT ISNULL(SUM(COI.FoodMenuQty*FMI.Consumption),0) FROM CustomerOrder CO INNER JOIN CustomerOrderItem COI ON COI.CustomerOrderID = CO.Id INNER JOIN FOODMENU FM ON FM.iD = coi.FOODMENUID INNER JOIN FoodMenuIngredient FMI ON FMI.Foodmenuid = FM.Id INNER JOIN Ingredient I ON I.ID = FMI.IngredientId

--SELECT * FROM CustomerOrderItem

--declare @id int
--set @id = 16

--DELETE FROM InventoryHistory WHERE Reference ='Purchase' AND ReferenceId=@ID

--INSERT INTO InventoryHistory ([StoreId],[IngredientId],[Reference],[ReferenceId],[StockDate],[StockInQty] ,[StockOutQty] ,[StockBalanceQty] ,[StockInRate]
--      ,[StockOutRate],[StockInAmount],[StockOutAmount],[Notes],[UserIdInserted],[IsDeleted])
--select P.StoreId,PRI.IngredientId,'Purchase',P.Id,P.PurchaseDate,PRI.Qty,0,0 as BalQty,PRI.UnitPrice,0,PRI.TotalAmount,0,Notes,1,0 from Purchase P
--INNER JOIN PurchaseIngredient PRI ON PRI.PurchaseId = P.Id AND PRI.IsDeleted=0
--WHERE P.Id = @id

--INSERT INTO Inventory ([StoreId],[IngredientId],[StockQty],[UserIdInserted],[IsDeleted])
--SELECT P.StoreId,PRI.IngredientId,PRI.Qty,1,0 from Purchase P
--INNER JOIN PurchaseIngredient PRI ON PRI.PurchaseId = P.Id AND PRI.IsDeleted=0
----WHERE P.Id = @id

DELETE FROM Inventory

INSERT INTO INVENTORY(StoreId,IngredientId,StockQty,UserIdInserteD,ISDELETED)

--select S.StoreName,I.IngredientName,
select S.Id,I.ID,
(
(SELECT ISNULL(SUM(PRI.QTY),0) FROM PurchaseIngredient PRI WHERE PRI.IngredientId = I.Id AND PRI.IsDeleted=0) -- Pur Stock In
+(SELECT ISNULL(SUM(IAI.IntgredientQty),0) FROM InventoryAdjustmentIngredient IAI WHERE IAI.IngredientId = I.Id AND IAI.ConsumptionStatus=1 AND IAI.IsDeleted=0) --Adj Storck IN
+(SELECT ISNULL(SUM(ITI.IntgredientQty),0) FROM InventoryTransferIngredient ITI INNER JOIN InventoryTransfer IT ON IT.ID = ITI.InventoryTransferId WHERE IT.ToStoreId = S.Id AND ITI.IngredientId = I.Id AND ITI.IsDeleted=0) -- Trans Stock IN
)
-
(
(SELECT ISNULL(SUM(IAI.IntgredientQty),0) FROM InventoryAdjustmentIngredient IAI WHERE IAI.IngredientId = I.Id AND IAI.ConsumptionStatus=2 AND IAI.IsDeleted=0) -- Adj Stock Out
+(SELECT ISNULL(SUM(ITI.IntgredientQty),0) FROM InventoryTransferIngredient ITI INNER JOIN InventoryTransfer IT ON IT.ID = ITI.InventoryTransferId WHERE IT.FromStoreId = S.Id AND ITI.IngredientId = I.Id AND ITI.IsDeleted=0) -- Trans Stock OUT
+(SELECT ISNULL(SUM(WI.IngredientQty),0) FROM WasteIngredient WI INNER JOIN Waste W ON W.ID = WI.WasteId INNER JOIN Outlet O ON O.Id =  W.OutletId INNER JOIN Store S ON S.Id = O.StoreId AND WI.IngredientId = I.Id AND WI.IsDeleted=0) -- Waste Stock OUT
+(SELECT ISNULL(SUM(COI.FoodMenuQty*FMI.Consumption),0) FROM CustomerOrder CO INNER JOIN CustomerOrderItem COI ON COI.CustomerOrderID = CO.Id INNER JOIN FOODMENU FM ON FM.iD = coi.FOODMENUID INNER JOIN FoodMenuIngredient FMI ON FMI.Foodmenuid = FM.Id INNER JOIN Ingredient I ON I.ID = FMI.IngredientId) -- Sales stock OUT
) AS StockQty,1,0
--,I.AlterQty

from Ingredient I
Cross join Store S 
WHERE S.ISDELETED=0

exec rptInventory

--select * from InventoryHistory
select * from Inventory
