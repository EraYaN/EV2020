c = 340; % speed of sound [m/s]
beta = 1;

tx = 1.2;
ty = 0.3;
rx = 3.1;
ry = 3.3;

r = sqrt((tx - rx)^2 + (ty - ry)^2);
loc = r / c;
result = beta / r^2; % The direct path from RX to TX
plot([loc loc], [0 result], '-');

hold on;

%plot(tx, ty, '*');
%plot(rx, ry, '*');

%xlabel('x location (m)');
%ylabel('y location (m)');

%plot([tx rx], [ty ry], 'r');

%plot([0 0], [0 4])
%plot([4 0], [4 4])
%plot([4 0], [0 0])
%plot([4 4], [4 0])

for x = 0:4
    for y = 1:4
        if x == y
           continue;
        end
        
        % Mirror the TX over the axes
        if x == 0 % Do not mirror
            t_x = tx;
            t_y = ty;
        elseif x == 1 % Mirror to the top side
            t_x = tx;
            t_y = 4 + (4 - ty);
        elseif x == 2 % Mirror to the right side
            t_x = 4 + (4 - tx);
            t_y = ty;
        elseif x == 3 % Mirror to the bottom side
            t_x = tx;
            t_y = -ty;
        else % Mirror to the left side
            t_x = -tx;
            t_y = ty;
        end
        
        % Mirror the RX over the axes
        if y == 1 % Mirror to the top side
            r_x = rx;
            r_y = 4 + (4 - ry);
        elseif y == 2 % Mirror to the right side
            r_x = 4 + (4 - rx);
            r_y = ry;
        elseif y == 3 % Mirror to the bottom side
            r_x = rx;
            r_y = -ry;
        else % Mirror to the left side
            r_x = -rx;
            r_y = ry;
        end
        
        % Connect the mirrored RX and RY
        dir = (r_y - t_y) / (r_x - t_x);
        y_0 = t_y - t_x * dir;
        x_0 = t_x - t_y / dir;
        syms y_l
        y_l = solve(y_l + dir * x_0 == 0);
        syms y_r
        y_r = solve(y_r / dir + x_0 == 4);
        syms x_t
        x_t = solve(y_0 + dir * x_t == 4);
        syms x_b
        x_b = solve(y_0 + dir * x_b == 0);
        
        %plot(r_x, r_y, '* r');
        %if x > 0
        %    plot(t_x, t_y, '* r');
        %end
        %plot(x_t, 4, '* r');
        %plot(x_b, 0, '* r');
        %plot(0, y_l, '* r');
        %plot(4, y_r, '* r');
        
        %plot([t_x r_x], [t_y r_y], ': r')
        
        bounces = 0;
        if x == 0 % Not mirrored
            p1 = [tx ty];
        elseif x == 1 % Was mirrored over the top side
            p1 = [x_t 4];
        elseif x == 2 % Was mirrored over the right side
            p1 = [4 y_r];
        elseif x == 3 % Was mirrored over the bottom side
            p1 = [x_b 0];
        elseif x == 4 % Was mirrored over the left side
            p1 = [0 y_l];
        end
        
        if p1(1) >= 0 && p1(1) <= 4 && p1(2) >= 0 && p1(2) <= 4
            %plot(p1(1), p1(2), '* r');
            bounces = bounces + 1;
        end
        
        if y == 1 % Was mirrored over the top side
            p2 = [x_t 4];
        elseif y == 2 % Was mirrored over the right side
            p2 = [4 y_r];
        elseif y == 3 % Was mirrored over the bottom side
            p2 = [x_b 0];
        elseif y == 4 % Was mirrored over the left side
            p2 = [0 y_l];
        end
        
        if p2(1) >= 0 && p2(1) <= 4 && p2(2) >= 0 && p2(2) <= 4
            %plot(p2(1), p2(2), '* r');
            bounces = bounces + 1;
        end
        
        if bounces == 2
            r = sqrt((tx - p1(1))^2 + (ry - p2(2))^2);
            r = r + sqrt((p1(1) - p2(1))^2 + (p1(2) - p2(2))^2);
            if x > 0
                r = r + sqrt((tx - p1(1))^2 + (ty - p1(2))^2);
            end
            
            result = result + beta / r^2;
            
            loc = r / c; % Calc the time to travel the distance r
            plot([loc loc], [0 beta/r^2], '-');
            
            %plot([tx p1(1)], [ty p1(2)], 'r');
            %plot([rx p2(1)], [ry p2(2)], 'r');
            %plot([p1(1) p2(1)], [p1(2) p2(2)], 'r');
        end
    end
end

disp(result)

hold off;