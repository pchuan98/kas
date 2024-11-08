/**
 * todos
 * 
 * 单个展缓
 */

import kaspa from "kaspa-chuan";
import { log } from "../utils/log";


// /**
//  *
//  * @param rpc
//  * @param key
//  * @param from
//  * @param to
//  * @param amount
//  */
// async function estimateFees(
//     rpc: kaspa.RpcClient,
//     key: string,
//     from: string,
//     to: string,
//     amount: number,
// ): {
//     const wallet = kaspa.Wallet({
//     networkId: "testnet-10",
//     encoding: kaspa.Encoding.Borsh,
// });
// }

// estimate fees

// using transaction

const rpc = new kaspa.RpcClient({
    url: "vpn.pchuan.site",
    networkId: "testnet-10",
    encoding: kaspa.Encoding.Borsh,
});

await rpc.connect();
const key = "40c67afc725a0ef35e7f6808cf24f88b4b7287a3861682fec4180d1a2511d57c";
const address = "kaspatest:qp7uq35snucwdl8xyz8cj2r4s56sa0w3wpp0e4lg0ge5lcsqp4crj97gwl4s3";
const charge = "kaspatest:qr74zkt02dsctjm9y8g4fmz2jlkgpdl7g6hemxmdnw8wsawqkpdmzmekfjvpx";

console.log(await rpc.getBalanceByAddress({ address }));
rpc.disconnect();
process.exit(0);

const context = new kaspa.UtxoContext({
    processor: new kaspa.UtxoProcessor({ rpc: rpc, networkId: "testnet-10" })
});
context.trackAddresses([address]);

const payload: kaspa.IPaymentOutput[] = [
    {
        address: charge,
        amount: kaspa.kaspaToSompi("100")!
    }
]

const utxos = (await rpc.getUtxosByAddresses([address])).entries;
utxos.sort((a, b) => {
    return Number(a.amount) - Number(b.amount);
});

const entry: kaspa.IUtxoEntry[] = utxos;

const fee: bigint = 0n;
const transaction = kaspa.createTransaction(entry, payload, fee);

console.log(kaspa.calculateTransactionFee("testnet-10", transaction));
console.log(kaspa.sompiToKaspaString(kaspa.calculateTransactionMass("testnet-10", transaction)));

const { transactions, summary } = await kaspa.createTransactions({
    outputs: payload,
    changeAddress: charge,
    priorityFee: kaspa.kaspaToSompi("0"),
    entries: context,
});

console.log(transactions.entries.length);

// console.log(kaspa.calculateTransactionFee("testnet-10", transactions[0].transaction));
// console.log(kaspa.sompiToKaspaString(kaspa.calculateTransactionMass("testnet-10", transactions[0].transaction)));


await rpc.disconnect();