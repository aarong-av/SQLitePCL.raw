/*
   Copyright 2014-2019 SourceGear, LLC

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

// Copyright © Microsoft Open Technologies, Inc.
// All Rights Reserved
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache 2 License for the specific language governing permissions and limitations under the License.

namespace SQLitePCL
{
    using System;

    // TODO dislike _low names
    public delegate int delegate_collation_low(object user_data, int len1, IntPtr p1, int len2, IntPtr p2);
    public delegate void delegate_update_low(object user_data, int type, IntPtr database, IntPtr table, long rowid);
    public delegate int delegate_trace_v2(uint t, object user_data, IntPtr p, IntPtr x);

    public delegate void delegate_log_low(object user_data, int errorCode, IntPtr msg);
    public delegate int delegate_authorizer_low(object user_data, int action_code, IntPtr param0, IntPtr param1, IntPtr dbName, IntPtr inner_most_trigger_or_view);
    public delegate int delegate_exec_low(object user_data, IntPtr[] values, IntPtr[] names);

    public delegate int delegate_commit(object user_data);
    public delegate void delegate_rollback(object user_data);

    public delegate int delegate_progress(object user_data);

    public delegate void delegate_function_scalar(sqlite3_context ctx, object user_data, sqlite3_value[] args);
    public delegate void delegate_function_aggregate_step(sqlite3_context ctx, object user_data, sqlite3_value[] args);
    public delegate void delegate_function_aggregate_final(sqlite3_context ctx, object user_data);

    /// <summary>
    ///
    /// This interface provides core functionality of the SQLite3 API.  It is the
    /// boundary between the portable class library and the platform-specific code
    /// below.
    ///
    /// In general, it is defined to be as low-level as possible while still remaninig
    /// "portable".  For example, a sqlite3 connection handle appears here as an IntPtr.
    /// Same goes for the C-level sqlite3_stmt pointer, also an IntPtr.
    ///
    /// This whole library is designed in 4 layers:
    ///
    /// (1)  The SQLite C API itself
    ///
    /// (2)  The declarations of the C API.  pinvoke.
    ///
    /// (3)  A C# layer in the platform assembly which implements this interface.
    ///
    /// (4)  The raw API, here in the PCL, which wraps an instance of this interface in
    ///      an API which replaces all the IntPtrs with strong typed (but still opaque) 
    ///      counterparts and converts strings to/from utf8..
    ///
    /// Even the top layer is still very low-level, which is why it is called "raw".
    /// This API is not intended to be used by app developers.  Rather it is designed
    /// to be a portable foundation for higher-level SQLite APIs.
    ///
    /// The philosophy of this library is to remain as similar to the underlying
    /// SQLite API as possible, even to the point of keeping the sqlite3_style_names
    /// and style.  It is expected that higher-level APIs built on this wrapper
    /// would present an API which is friendlier to C# developers.
    ///
    /// </summary>
    public interface ISQLite3Provider
    {
        int sqlite3_open(IntPtr filename, out IntPtr db);
        int sqlite3_open_v2(IntPtr filename, out IntPtr db, int flags, IntPtr vfs);
        int sqlite3_close_v2(IntPtr db); /* 3.7.14+ */
        int sqlite3_close(IntPtr db);

        int sqlite3_enable_shared_cache(int enable);

        void sqlite3_interrupt(sqlite3 db);

        int sqlite3__vfs__delete(IntPtr vfs, IntPtr pathname, int syncDir);

        int sqlite3_threadsafe();
        IntPtr sqlite3_libversion();
        int sqlite3_libversion_number();
        IntPtr sqlite3_sourceid();
        long sqlite3_memory_used();
        long sqlite3_memory_highwater(int resetFlag);
        int sqlite3_status(int op, out int current, out int highwater, int resetFlag);

        int sqlite3_db_readonly(sqlite3 db, IntPtr dbName);
        IntPtr sqlite3_db_filename(sqlite3 db, IntPtr att);
        IntPtr sqlite3_errmsg(sqlite3 db);
        long sqlite3_last_insert_rowid(sqlite3 db);
        int sqlite3_changes(sqlite3 db);
        int sqlite3_total_changes(sqlite3 db);
        int sqlite3_get_autocommit(sqlite3 db);
        int sqlite3_busy_timeout(sqlite3 db, int ms);

        int sqlite3_extended_result_codes(sqlite3 db, int onoff);
        int sqlite3_errcode(sqlite3 db);
        int sqlite3_extended_errcode(sqlite3 db);
        IntPtr sqlite3_errstr(int rc); /* 3.7.15+ */

        int sqlite3_prepare_v2(sqlite3 db, IntPtr sql, out IntPtr stmt, out IntPtr remain);
        int sqlite3_prepare_v3(sqlite3 db, IntPtr sql, uint flags, out IntPtr stmt, out IntPtr remain);
        int sqlite3_step(sqlite3_stmt stmt);
        int sqlite3_finalize(IntPtr stmt);
        int sqlite3_reset(sqlite3_stmt stmt);
        int sqlite3_clear_bindings(sqlite3_stmt stmt);
        int sqlite3_stmt_status(sqlite3_stmt stmt, int op, int resetFlg);
        IntPtr sqlite3_sql(sqlite3_stmt stmt);
        IntPtr sqlite3_db_handle(IntPtr stmt);
        IntPtr sqlite3_next_stmt(sqlite3 db, IntPtr stmt);

        int sqlite3_bind_zeroblob(sqlite3_stmt stmt, int index, int size);
        IntPtr sqlite3_bind_parameter_name(sqlite3_stmt stmt, int index);
        int sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte[] blob);
        int sqlite3_bind_blob(sqlite3_stmt stmt, int index, byte[] blob, int nSize);
        int sqlite3_bind_double(sqlite3_stmt stmt, int index, double val);
        int sqlite3_bind_int(sqlite3_stmt stmt, int index, int val);
        int sqlite3_bind_int64(sqlite3_stmt stmt, int index, long val);
        int sqlite3_bind_null(sqlite3_stmt stmt, int index);
        int sqlite3_bind_text(sqlite3_stmt stmt, int index, IntPtr text);
        int sqlite3_bind_parameter_count(sqlite3_stmt stmt);
        int sqlite3_bind_parameter_index(sqlite3_stmt stmt, IntPtr strName);

        IntPtr sqlite3_column_database_name(sqlite3_stmt stmt, int index);
        IntPtr sqlite3_column_name(sqlite3_stmt stmt, int index);
        IntPtr sqlite3_column_origin_name(sqlite3_stmt stmt, int index);
        IntPtr sqlite3_column_table_name(sqlite3_stmt stmt, int index);
        IntPtr sqlite3_column_text(sqlite3_stmt stmt, int index);
        int sqlite3_data_count(sqlite3_stmt stmt);
        int sqlite3_column_count(sqlite3_stmt stmt);
        double sqlite3_column_double(sqlite3_stmt stmt, int index);
        int sqlite3_column_int(sqlite3_stmt stmt, int index);
        long sqlite3_column_int64(sqlite3_stmt stmt, int index);
        ReadOnlySpan<byte> sqlite3_column_blob(sqlite3_stmt stmt, int index);
        int sqlite3_column_blob(sqlite3_stmt stm, int columnIndex, byte[] result, int offset);
        int sqlite3_column_bytes(sqlite3_stmt stmt, int index);
        int sqlite3_column_type(sqlite3_stmt stmt, int index);
        IntPtr sqlite3_column_decltype(sqlite3_stmt stmt, int index);

        sqlite3_backup sqlite3_backup_init(sqlite3 destDb, IntPtr destName, sqlite3 sourceDb, IntPtr sourceName);
        int sqlite3_backup_step(sqlite3_backup backup, int nPage);
        int sqlite3_backup_remaining(sqlite3_backup backup);
        int sqlite3_backup_pagecount(sqlite3_backup backup);
        int sqlite3_backup_finish(IntPtr backup);

        int sqlite3_blob_open(sqlite3 db, byte[] db_utf8, byte[] table_utf8, byte[] col_utf8, long rowid, int flags, out sqlite3_blob blob);
        int sqlite3_blob_bytes(sqlite3_blob blob);
        int sqlite3_blob_reopen(sqlite3_blob blob, long rowid);
        int sqlite3_blob_write(sqlite3_blob blob, byte[] b, int bOffset, int n, int offset);
        int sqlite3_blob_read(sqlite3_blob blob, byte[] b, int bOffset, int n, int offset);
        int sqlite3_blob_close(IntPtr blob);

        int sqlite3_config_log(delegate_log_low func, object v);
        void sqlite3_commit_hook(sqlite3 db, delegate_commit func, object v);
        void sqlite3_rollback_hook(sqlite3 db, delegate_rollback func, object v);

        int sqlite3_trace_v2(sqlite3 db, uint mask, delegate_trace_v2 func, object v);

        void sqlite3_progress_handler(sqlite3 db, int instructions, delegate_progress func, object v);
        void sqlite3_update_hook(sqlite3 db, delegate_update_low func, object v);
        int sqlite3_create_collation(sqlite3 db, byte[] name, object v, delegate_collation_low func);
        int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, object v, delegate_function_scalar func);
        int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final);
        int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_scalar func);
        int sqlite3_create_function(sqlite3 db, byte[] name, int nArg, int flags, object v, delegate_function_aggregate_step func_step, delegate_function_aggregate_final func_final);

        int sqlite3_db_status(sqlite3 db, int op, out int current, out int highest, int resetFlg);

        void sqlite3_result_blob(IntPtr context, byte[] val);
        void sqlite3_result_double(IntPtr context, double val);
        void sqlite3_result_error(IntPtr context, IntPtr strErr);
        void sqlite3_result_int(IntPtr context, int val);
        void sqlite3_result_int64(IntPtr context, long val);
        void sqlite3_result_null(IntPtr context);
        void sqlite3_result_text(IntPtr context, IntPtr val);
        void sqlite3_result_zeroblob(IntPtr context, int n);
        // TODO sqlite3_result_value
        void sqlite3_result_error_toobig(IntPtr context);
        void sqlite3_result_error_nomem(IntPtr context);
        void sqlite3_result_error_code(IntPtr context, int code);

        byte[] sqlite3_value_blob(IntPtr p);
        int sqlite3_value_bytes(IntPtr p);
        double sqlite3_value_double(IntPtr p);
        int sqlite3_value_int(IntPtr p);
        long sqlite3_value_int64(IntPtr p);
        int sqlite3_value_type(IntPtr p);
        IntPtr sqlite3_value_text(IntPtr p);

        int sqlite3_stmt_busy(sqlite3_stmt stmt);
        int sqlite3_stmt_readonly(sqlite3_stmt stmt);

        int sqlite3_exec(sqlite3 db, IntPtr sql, delegate_exec_low callback, object user_data, out IntPtr errMsg);

        int sqlite3_complete(IntPtr sql);

        int sqlite3_compileoption_used(IntPtr sql);
        IntPtr sqlite3_compileoption_get(int n);

        int sqlite3_wal_autocheckpoint(sqlite3 db, int n);
        int sqlite3_wal_checkpoint(sqlite3 db, IntPtr dbName);
        int sqlite3_wal_checkpoint_v2(sqlite3 db, IntPtr dbName, int eMode, out int logSize, out int framesCheckPointed);

        int sqlite3_table_column_metadata(sqlite3 db, IntPtr dbName, IntPtr tblName, IntPtr colName, out IntPtr dataType, out IntPtr collSeq, out int notNull, out int primaryKey, out int autoInc);

        int sqlite3_set_authorizer(sqlite3 db, delegate_authorizer_low authorizer, object user_data);

        int sqlite3_stricmp(IntPtr p, IntPtr q);
        int sqlite3_strnicmp(IntPtr p, IntPtr q, int n);

        void sqlite3_free(IntPtr p);

#if not // TODO consider these
        int sqlite3_key(sqlite3 db, byte[] key, int keylen);
        int sqlite3_rekey(sqlite3 db, byte[] key, int keylen);

        int sqlite3_load_extension(sqlite3 db, IntPtr fileName, IntPtr procName, ref IntPtr pError);
#endif

        int sqlite3_initialize();
        int sqlite3_shutdown();

        // sqlite3_config() takes a variable argument list
        int sqlite3_config(int op);
        int sqlite3_config(int op, int val);

        int sqlite3_enable_load_extension(sqlite3 db, int enable);


        int sqlite3_win32_set_directory(int typ, IntPtr path);
    }
}

