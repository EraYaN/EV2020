classdef Communication < handle
    %COMMUNICATION Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        port;        
        bufferSize = 50000; 
        term = char(10);
    end
    
    methods
        function Com = Communication(portName)
            Com.port = serial(portName);
            Com.port.InputBufferSize = Com.bufferSize;
            Com.port.OutputBufferSize = Com.bufferSize;
            Com.port.BaudRate = 115200;
            Com.port.FlowControl = 'hardware';
            Com.port.RequestToSend = 'on';
            Com.port.DataTerminalReady = 'off';
            Com.port.DataBits = 8;
            Com.port.StopBits = 1;
            Com.port.Terminator = 'LF'; 
            Com.port.Name = 'CommunicationPort';
            out = instrfind('Port',Com.port);
            if(~isempty(out))
                disp('WARNING: Port in use. Closing.');
                if(~strcmp(get(out(1),'Status'),'open'))
                    delete(out(1));
                else
                    fclose(out(1));
                    delete(out(1));
                end
            end
            fopen(Com.port);
        end        
        function drive(Com,drive,steering)
            if nargin<1
                drive = 150;
            end
            if nargin<2
                steering = 150;
            end
            steering = clamp(steering,100,200);
            drive = clamp(drive,135,165);
            fprintf(Com.port,'D%1$3u %2$3u',steering,drive);
        end
        function [drive,steering,left,right,battery,audio] = status(Com) 
            fprintf(Com.port,'S');
            buffer = fgetl(Com.port);
            [steering, drive]=sscanf(buffer,'D%d %d', 2);
            buffer = fgetl(Com.port);
            [left, right]= sscanf(buffer,'U%d %d', 2);
            buffer = fgetl(Com.port);
            battery=sscanf(buffer,'A%d', 1);
            buffer = fgetl(Com.port);
            audio=sscanf(buffer,'Audio %d', 1);
        end
        function audio(Com,status)
            if nargin<1
                status = 0;
            end            
            status = clamp(status,0,1);
            fprintf(Com.port,'A%1$1u',status);
        end
        function kill(Com)
            Com.drive();
        end
        function delete(Com)
           fclose(Com.port);           
        end
    end
    
end

