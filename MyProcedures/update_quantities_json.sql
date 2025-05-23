CREATE OR REPLACE PROCEDURE public.update_quantities_json(
    IN p_updates json
)
LANGUAGE plpgsql
AS $$
DECLARE
    item JSON;
    ordered_item_id INT;
    d_quantity INT;
BEGIN
    FOR item IN SELECT * FROM json_array_elements(p_updates)
    LOOP
        ordered_item_id := (item ->> 'ordered_item_id')::INT;
        d_quantity := (item ->> 'quantity')::INT;

        UPDATE ordereditems
        SET readyquantity = readyquantity - d_quantity
        WHERE ordereditemid = ordered_item_id;
    END LOOP;
END;
$$;
