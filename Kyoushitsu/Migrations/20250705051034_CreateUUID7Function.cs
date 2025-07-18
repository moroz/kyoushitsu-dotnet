﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kyoushitsu.Migrations
{
    /// <inheritdoc />
    public partial class CreateUUID7Function : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				CREATE EXTENSION IF NOT EXISTS pgcrypto;

				CREATE OR REPLACE FUNCTION uuid7() RETURNS uuid AS $$
				DECLARE
				BEGIN
					RETURN uuid7(clock_timestamp());
				END $$ LANGUAGE plpgsql;

				CREATE OR REPLACE FUNCTION uuid7(p_timestamp timestamp with time zone) RETURNS uuid AS $$
				DECLARE
					v_time double precision := null;

					v_unix_t bigint := null;
					v_rand_a bigint := null;
					v_rand_b bigint := null;

					v_unix_t_hex varchar := null;
					v_rand_a_hex varchar := null;
					v_rand_b_hex varchar := null;

					c_milli double precision := 10^3;  -- 1 000
					c_micro double precision := 10^6;  -- 1 000 000
					c_scale double precision := 4.096; -- 4.0 * (1024 / 1000)

					c_version bigint := x'0000000000007000'::bigint; -- RFC-9562 version: b'0111...'
					c_variant bigint := x'8000000000000000'::bigint; -- RFC-9562 variant: b'10xx...'
				BEGIN
					v_time := extract(epoch from p_timestamp);

					v_unix_t := trunc(v_time * c_milli);
					v_rand_a := trunc((v_time * c_micro - v_unix_t * c_milli) * c_scale);
					v_rand_b := trunc(random() * 2^30)::bigint << 32 | trunc(random() * 2^32)::bigint;

					v_unix_t_hex := lpad(to_hex(v_unix_t), 12, '0');
					v_rand_a_hex := lpad(to_hex((v_rand_a | c_version)::bigint), 4, '0');
					v_rand_b_hex := lpad(to_hex((v_rand_b | c_variant)::bigint), 16, '0');

					RETURN (v_unix_t_hex || v_rand_a_hex || v_rand_b_hex)::uuid;
				END $$ LANGUAGE plpgsql;
				");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DROP FUNCTION IF EXISTS uuid7(timestamp with time zone);
				DROP FUNCTION IF EXISTS uuid7();
				DROP EXTENSION IF EXISTS pgcrypto;
			");
        }
    }
}
