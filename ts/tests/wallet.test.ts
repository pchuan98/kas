import { createWallet } from "../core/wallet";
import { expect, test } from "bun:test";

test("create wallet", async () => {
    const mnemonic = await createWallet();
    
    
    expect(mnemonic).toBeString();
    expect(mnemonic).toBeDefined();
});
