import { expect, test } from "bun:test";
import kaspa from "../wasm/kaspa";

const url = "vpn.pchuan.site";

const client = new kaspa.RpcClient(
    {
        url,
        encoding: kaspa.Encoding.Borsh,
        networkId: "testnet-10",
    });

await client.connect();

test("connect", async () => {
    client.getCoinSupply();
    expect(client).toBeDefined();
});