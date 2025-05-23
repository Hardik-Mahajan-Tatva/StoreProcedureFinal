CREATE OR REPLACE FUNCTION sp_get_kot_data(
    page_number INT,
    page_size INT,
    input_category_id INT,
    input_order_status TEXT,
    input_item_status TEXT
)
RETURNS SETOF kot_flat_data
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        o.orderid,
        o.createdat,
        o.orderwisecomment,
        t.tablename::TEXT,
        s.sectionname::TEXT,
        oi.ordereditemid,
        i.itemid,
        i.itemname,
        oi.quantity,
        oi.readyquantity,
        oi.itemwisecomment,
        i.rate,
        im.modifiername::TEXT,
        im.rate
    FROM "orders" o
    INNER JOIN ordereditems oi ON o.orderid = oi.orderid
    INNER JOIN items i ON oi.itemid = i.itemid
    LEFT JOIN ordereditemmodifer oim ON oi.ordereditemid = oim.ordereditemid
    LEFT JOIN modifiers im ON oim.itemmodifierid = im.modifierid
    LEFT JOIN ordertable otm ON o.orderid = otm.orderid
    LEFT JOIN "tables" t ON otm.tableid = t.tableid
    LEFT JOIN sections s ON t.sectionid = s.sectionid
    WHERE 
        (input_category_id = 0 OR i.categoryid = input_category_id)
        AND (
            input_order_status = '' OR
            (input_order_status = 'InProgress' AND o.status = 1 AND (oi.quantity - oi.readyquantity) > 0) OR
            (input_order_status = 'Ready' AND o.status = 1 AND oi.readyquantity > 0)
        )
        AND (
            input_item_status = '' OR
            (input_item_status = 'InProgress' AND (oi.quantity - oi.readyquantity) > 0) OR
            (input_item_status = 'Ready' AND oi.readyquantity > 0)
        )
    ORDER BY o.orderid
    OFFSET (page_number - 1) * page_size
    LIMIT page_size;
END;
$$;


SELECT * FROM sp_get_kot_data(1, 10, 0, '', '');
