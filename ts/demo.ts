import kaspa from "kaspa-chuan";
import { env, serve } from 'bun';
import { log } from "./utils/log"

setTimeout(() => {
	process.exit(0);
}, 2000);

const client = new kaspa.RpcClient(
	{
		url: "vpn.pchuan.site",
		encoding: kaspa.Encoding.Borsh,
		networkId: "testnet-10",
	});

await client.disconnect();
await client.connect();

log.info(`Connected to ${client}`);

if (!env.KAS_PRIVATE_KEY) {
	throw new Error("KAS_PRIVATE_KEY is not defined");
}

const key: string = String(env.KAS_PRIVATE_KEY);

const pkey = new kaspa.PrivateKey(key);
log.info(`Private key: ${pkey.toString()}`);

const pubkey = pkey.toPublicKey();
log.info(`Public key: ${pubkey.toString()}`);

const address = pubkey.toAddress("testnet-10");
log.info(`Address: ${address.toString()}`);

const info = await client.getInfo();
log.info({
	"p2pid": info.p2pId,
	"poolsize": info.mempoolSize,
	"version": info.serverVersion,
	"is_utxo": info.isUtxoIndexed,
	"is_sync": info.isSynced,
});

const sender = new kaspa.tran("testnet-10", pkey, client);

await client.disconnect();
log.info("Disconnected");