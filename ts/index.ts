import kaspa from "./wasm/kaspa";
import { serve } from 'bun';

serve({
    fetch(req) {
        return new Response('Hello, Bun!', { status: 200 });
    },
    port: 3001,
});


const client = new kaspa.RpcClient(
    {
        url: "127.0.0.1",
        encoding: kaspa.Encoding.Borsh,
        networkId: "testnet-10",
    });

await client.connect();
console.log(client);

const balance = await client.getBalanceByAddress({
    address: "kaspatest:qp7uq35snucwdl8xyz8cj2r4s56sa0w3wpp0e4lg0ge5lcsqp4crj97gwl4s3",
});

console.log(balance.balance);

await new Promise(() => {
    setTimeout(() => {
        process.exit(0);
    }, 3000);
});