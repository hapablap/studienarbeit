//============================================================================
// Name        : DataFetch.cpp
// Author      : Daniel Faehrmann
// Version     : 1.0
//============================================================================

#include <iostream>
#include <fstream>
#include <conio.h>
#include <sstream>
#include <windows.h>
#include <map>

#include "include/EmoStateDLL.h"
#include "include/edk.h"
#include "include/edkErrorCode.h"

EE_DataChannel_t targetChannelList[] = {
	ED_COUNTER,
	ED_AF3, ED_F7, ED_F3, ED_FC5, ED_T7,
	ED_P7, ED_O1, ED_O2, ED_P8, ED_T8,
	ED_FC6, ED_F4, ED_F8, ED_AF4, ED_GYROX, ED_GYROY, ED_TIMESTAMP,
	ED_FUNC_ID, ED_FUNC_VALUE, ED_MARKER, ED_SYNC_SIGNAL
};

const char header[] = "COUNTER,AF3,F7,F3, FC5, T7, P7, O1, O2,P8"
                      ", T8, FC6, F4,F8, AF4,GYROX, GYROY, TIMESTAMP, "
                      "FUNC_ID, FUNC_VALUE, MARKER, SYNC_SIGNAL,";

int main(int argc, char** argv) {

	EmoEngineEventHandle eEvent	= EE_EmoEngineEventCreate();
	EmoStateHandle eState		= EE_EmoStateCreate();
	unsigned int userID			= 0;
	float secs					= 1;
	bool readytocollect			= false;
	int option					= 0;
	int state					= 0;

	std::string input;

	try {
		if (argc != 2) {
			std::cout << "Please supply the log file name.\nUsage: EEGLogger [log_file_name]." << std::endl;
			throw std::exception();
		}

		std::cout << "===================================================================" << std::endl;
		std::cout << "Log EEG Data from EmoEngine" << std::endl;
		std::cout << "===================================================================" << std::endl;
		std::cout << "Press '1' to start and connect to the EmoEngine                    " << std::endl;
		std::cout << ">> ";

		std::getline(std::cin, input, '\n');
		option = atoi(input.c_str());

		switch (option) {
			case 1:
			{
				if (EE_EngineConnect() != EDK_OK) {
					std::cout << "Emotiv Engine start up failed." << std::endl;
					throw std::exception();
				}
				break;
			}
			default:
				std::cout << "Invalid option..." << std::endl;
				throw std::exception();
				break;
		}

		std::cout << "Start receiving EEG Data! Press any key to stop logging...\n" << std::endl;
    	std::ofstream ofs(argv[1],std::ios::trunc);
		ofs << header << std::endl;

		DataHandle hData = EE_DataCreate();
		EE_DataSetBufferSizeInSec(secs);

		std::cout << "Buffer size in secs:" << secs << std::endl;

		// _kbhit() checks whether key has been pressed
		while (!_kbhit()) {
			state = EE_EngineGetNextEvent(eEvent);

			if (state == EDK_OK) {
				EE_Event_t eventType = EE_EmoEngineEventGetType(eEvent);
				EE_EmoEngineEventGetUserId(eEvent, &userID);

				// Log the EmoState if it has been updated
				if (eventType == EE_UserAdded) {
					std::cout << "User added";
					EE_DataAcquisitionEnable(userID,true);
					readytocollect = true;
				}
			}

			if (readytocollect) {
				EE_DataUpdateHandle(0, hData);

				unsigned int nSamplesTaken=0;
				EE_DataGetNumberOfSample(hData,&nSamplesTaken);

				std::cout << "Updated " << nSamplesTaken << std::endl;

				if (nSamplesTaken != 0) {
					double* data = new double[nSamplesTaken];
					for (int sampleIdx=0 ; sampleIdx<(int)nSamplesTaken ; ++ sampleIdx) {
						for (int i = 0 ; i<(int)(sizeof(targetChannelList)/sizeof(EE_DataChannel_t)) ; i++) {

							EE_DataGet(hData, targetChannelList[i], data, nSamplesTaken);
							ofs << data[sampleIdx] << ",";
						}
						ofs << std::endl;
					}
					delete[] data;
				}
			}
			Sleep(100);
		}
		ofs.close();
		EE_DataFree(hData);

	}
	catch (const std::exception& e) {
		std::cerr << e.what() << std::endl;
		std::cout << "Press any key to exit..." << std::endl;
		getchar();
	}

	EE_EngineDisconnect();
	EE_EmoStateFree(eState);
	EE_EmoEngineEventFree(eEvent);

	return 0;
}


