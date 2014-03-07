classdef Communication < handle
    %Communication Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        fid
    end
    
    methods
        
        function com = Communication(portName)
            %this = this@handle(ArgumentList);
            
            com.fid = serial(portName);
            com.fid.InputBufferSize = 50000;
            com.fid.OutputBufferSize = 50000;
            com.fid.FlowControl = 'hardware';
            com.fid.RequestToSend = 'on';
            com.fid.DataTerminalReady = 'off';
            com.fid.BaudRate = 115200;
            com.fid.Terminator = 'LF';
            fopen(com.fid);
            
            %while TRUE
                %sendDrive(port, 160, 160);
                %getResponse(this.port);
            %end
            
            %fclose(port);
        end
        
        function sendDrive(com, steering, driving)
           if steering < 135
               steering = 135;
           end
           if steering > 165
               steering = 165;
           end
           if driving < 135
               driving = 135;
           end
           if driving > 165
               driving = 165;
           end
           command = sprintf('D%1$3u %1$3u', steering, driving);
           
           com.sendCommand(command);
        end
        
        function sendCommand(com, command)
            fprintf(com.fid, command);
        end
        
        function [steering, driving, distanceLeft, distanceRight, battery, audio] = getStatus(com)
            com.sendCommand('S');
            
            %[steering,driving] = fscanf(com.fid, ['D%d %d' char(10)], 2);
            %[distanceLeft,distanceRight] = fscanf(com.fid, ['U%d %d' char(10)], 2);
            %battery = fscanf(com.fid,['A%d' char(10)], 1);
            %audio = fscanf(com.fid,['Audio %d' char(10)], 1);
            
            steeringDriving = fgetl(com.fid);
            [steering, driving] = sscanf(steeringDriving, 'D%d %d');
            
            distance = fgetl(com.fid);
            [distanceLeft, distanceRight] = sscanf(distance, 'U%d %d');
            
            battery = fgetl(com.fid);
            battery = sscanf(battery, 'A%d');
            
            audio = fgetl(com.fid);
            audio = sscanf(audio, 'Audio %d');
        end
        
        function delete (com)
           fclose(com.fid);
        end
        
    end
    
end

