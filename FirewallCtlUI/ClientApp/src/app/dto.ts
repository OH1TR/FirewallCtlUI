
declare namespace FirewallCtlUI.DTO {
	interface CaptureDevice {
		description: string;
		name: string;
	}
	interface Settings {
		device: string;
		myNetworks: string[];
	}
}
