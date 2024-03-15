-- Table: public.Execution

-- DROP TABLE IF EXISTS public."Execution";

CREATE TABLE IF NOT EXISTS public."Execution"
(
    "idExecution" integer NOT NULL DEFAULT nextval('"Execution_idExecution_seq"'::regclass),
    "WPM" integer,
    "KeystrokesQtde" integer,
    "Accuracy" integer,
    "CorrectWords" integer,
    "WrongWords" integer
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Execution"
    OWNER to postgres;