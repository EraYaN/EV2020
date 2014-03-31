classdef Communication < Handle
    %Communication Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        
    end
    
    methods
        
        function this = Communication(port)
            this = this@Handle(ArgumentList);
            
            this.port = serial(port);
            this.port.InputBufferSize = 50000;
            this.port.OutputBufferSize = 50000;
            this.port.FlowControl = 'hardware';
            this.port.RequestToSend = 'on';
            this.port.DataTerminalReady = 'off';
            this.port.BaudRate = 115200;
            fopen(this.port);
            
            while TRUE
                sendDrive(this.port, 160, 160);
                %getResponse(this.port);
            end
            
            fclose(this.port);
        end
        
        function sendDrive(port, steering, driving)
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
           command = 'D' + num2string(steering) + num2string(driving);
           
           sendCommand(port, command);
        end
        
        function sendCommand(port, command)
            set(port, command + '\n');
        end
        
        function [driving, steering, distanceLeft, distanceRight, battery, audio] = getStatus(port)
            response = fgetl(port);
            if response ~= NULL
                driving = substring(response, 2, 3);
                steering = substring(response, 4, 4);
            else
                driving = -1;
                steering = -1;
            end
            
            response = fgetl(port);
            if response ~= NULL
                distanceLeft = substring(response, 2, 3);
                distanceRight = substring(response, 4, 4);
            else
                distanceLeft = -1;
                distanceRight = -1;
            end
            
            response = fgetl(port);
            if response ~= NULL
                battery = substring(response, 2, 5);
            else
                battery = -1;
            end
            
            response = fgetl(port);
            if response ~= NULL
                audio = substring(response, 7, 1);
            else
                audio = -1;
            end
        end
        
    end
    
end

