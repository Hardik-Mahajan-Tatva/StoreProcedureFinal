
CREATE OR REPLACE PROCEDURE update_ready_quantities_json(p_updates JSON)
LANGUAGE plpgsql
AS $$
DECLARE
    item JSON;
    ordered_item_id INT;
    ready_quantity INT;
BEGIN
    FOR item IN SELECT * FROM json_array_elements(p_updates)
    LOOP
        ordered_item_id := (item ->> 'ordered_item_id')::INT;
        ready_quantity := (item ->> 'ready_quantity')::INT;


        UPDATE ordereditems
        SET readyquantity = readyquantity + ready_quantity
        WHERE ordereditemid = ordered_item_id;
    END LOOP;
END;
$$;
