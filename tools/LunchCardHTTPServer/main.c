/*
 * Simple HTTP server that should process both POST and GET request (only POST tested).
 * It was developed for the purpose of poll agent tests of Lunch Card for WP7.
 * It uses following lib and versions:
 *   - libsoup-dev_2.26.3-1_win32.zip, libsoup_2.26.3-1_win32.zip,
 *   - glib_2.34.3-1_win32.zip, glib-dev_2.34.3-1_win32.zip.
 * As you can see it was developed on Windwos 7 platform.
 * Ofcourse, I don't give any warranty that it will work and do no harm, etc.
 * Use on your own risk.
 */

#include <stdlib.h>
#include <glib.h>
#include <glib/gprintf.h>
#include <libsoup/soup.h>

const double MAX_BALANCE = 90.50;

/*
 * expected request
 * "action=mobileweb&code=1234&cardNumber=0123456789&lang=pl"
 * excpected response
 * "{\"cardname\":\"Ticket Restaurant\\u00ae\",\"balance\":{\"0123456789\":{\"amount\":5.65}}}"
 */

#if DEBUG
void iterate_hash(gpointer key, gpointer value, gpointer user_data)
{
    g_print("%s => %s\n", (gchar*) key, (gchar*)value);
}

void itarate_header(const char *name,
                    const char *value,
                    gpointer user_data)
{
    g_print("%s => %s\n", name, value);
}
#endif

void
server_callback (SoupServer        *server,
                 SoupMessage       *msg,
                 const char        *path,
                 GHashTable        *query,
                 SoupClientContext *client,
                 gpointer           user_data)
{
    gchar* code    = NULL;
    gchar* card_no = NULL;
    static gfloat balance = -1;


#if DEBUG
    g_print("%s\n", path);
    g_print("%s\n", msg->method);
    g_print("%d\n", msg->status_code);
    g_print("%s\n", msg->reason_phrase);
    g_print("%s\n", msg->request_body->data);

    g_print("request headers\n");
    soup_message_headers_foreach(msg->request_headers, itarate_header, NULL);
#endif

    if(!query)
    {
        if(msg->request_body->length > 0)
        {
            g_assert(msg->request_body->data);
            query = soup_form_decode(msg->request_body->data);
        }
    }

    if(query)
    {
#if DEBUG
        g_hash_table_foreach(query, iterate_hash, NULL);
#endif
        gpointer p_code    = g_hash_table_lookup(query, "code");
        gpointer p_card_no = g_hash_table_lookup(query, "cardNumber");
        code    = p_code;
        card_no = p_card_no;
    }
    else
    {
        g_print("Error! Parsing params failed!");
        if(msg && msg->request_body && msg->request_body && msg->response_body->data )
            g_print("Request: %s\n\n", msg->request_body->data);
        return;
    }

    if(!code || !card_no)
    {
        g_print("Error! Parsing params failed!");
        return;
    }

    gchar* resp_body = g_new(gchar, 1024);
    g_assert(resp_body);

    balance -= 30;
    if(balance < 0)
        balance = MAX_BALANCE;

    gsize resp_length = g_sprintf(resp_body,
        "{\"cardname\":\"Ticket Restaurant\\u00ae\",\"balance\":{\"%s\":{\"amount\":%.02f}}}\n",
        card_no, balance);


    soup_message_set_status (msg, SOUP_STATUS_OK);
    soup_message_set_response (msg, "application/json",
                               SOUP_MEMORY_TAKE,
                               resp_body,
                               resp_length);

#if DEBUG
    g_print("response: %s\n", resp_body);
#endif
}

int
main(int argc, char* argv[])
{
    SoupServer *server;
    guint port = 80;

    g_type_init();

    server = soup_server_new(SOUP_SERVER_PORT, port, NULL);
    if(!server)
    {
        g_print("Server is not initialized correctly!\n");
        return 1;
    }
    g_assert(server);

    soup_server_add_handler (server, "/", server_callback,
        NULL, NULL);

    g_print ("\nWaiting for requests on port %d...\n", port);

    soup_server_run(server);

    return 0;
}
